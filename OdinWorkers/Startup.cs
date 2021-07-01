using System.ComponentModel;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OdinWorkers.Models;
using OdinWorkers.Workers.RabbitMQWorker;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SqlSugar;
using SqlSugar.IOC;
using OdinPlugs.OdinCore.ConfigModel;
using OdinPlugs.OdinMvcCore.OdinInject;
using OdinPlugs.OdinNetCore.OdinSnowFlake.SnowFlakeInterface;
using OdinPlugs.OdinNetCore.OdinSnowFlake.SnowFlakeModel;
using OdinPlugs.OdinMAF.OdinMongoDb;
using OdinPlugs.OdinMAF.OdinCacheManager;
using OdinPlugs.OdinMvcCore.MvcCore;
using OdinPlugs.OdinMAF.OdinCapService;
using OdinPlugs.OdinMAF.OdinRedis;
using OdinPlugs.OdinMAF.OdinSerilog;
using OdinPlugs.OdinMAF.OdinSerilog.Models;

namespace OdinWorkers
{
    public class Startup
    {
        private IOptionsSnapshot<ProjectExtendsOptions> _iOptions;
        private ProjectExtendsOptions _Options;
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            var config = new ConfigurationBuilder()
               .AddEnvironmentVariables(prefix: "ASPNETCORE_")
               .Add(new JsonConfigurationSource { Path = "serverConfig/cnf.json", Optional = false, ReloadOnChange = true })
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddEnvironmentVariables();

            // ~ 按需加载对应项目环境的config
            // ^ 需要注意的是，如果多个配置文件有相同的配置信息，那么后加载的配置文件会覆盖先加载的配置文件(必须是.json格式的配置文件)
            // ~ 按运行环境 加载对应配置文件 
            // ~ 递归serviceConfig文件夹内所有的配置文件 加载及 cnf.config文件 以外的所有配置,
            var env = config.Build().GetValue<string>("ProjectConfigOptions:EnvironmentName");
            var rootPath = Directory.GetCurrentDirectory();
            ConfigLoadHelper.LoadConfigs(env, Path.Combine(rootPath, "serverConfig"), config, rootPath);
            Configuration = config.Build();


        }

        public void ConfigureServices(IServiceCollection services)
        {
            Assembly ass = Assembly.Load("OdinPlugs");


            Log.Logger.Information("启用【 强类型配置文件 】");
            services.Configure<ProjectExtendsOptions>(Configuration.GetSection("ProjectConfigOptions"));
            var provider = services.BuildServiceProvider();
            _iOptions = provider.GetRequiredService<IOptionsSnapshot<ProjectExtendsOptions>>();
            _Options = _iOptions.Value;
            services.AddSingleton<ProjectExtendsOptions>(_Options);

            services.AddOdinSingletonWithParamasInject<IOdinSnowFlake, OdinSnowFlakeOption>(
                ass,
                opt =>
                {
                    opt.DatacenterId = _Options.FrameworkConfig.SnowFlake.DataCenterId;
                    opt.WorkerId = _Options.FrameworkConfig.SnowFlake.WorkerId;
                });

            services
                .AddOdinTransientInject(this.GetType().Assembly)
                .AddOdinTransientInject(ass)
                .AddOdinTransientWithParamasInject<IOdinMongo>(ass, new Object[] { _Options.MongoDb.MongoConnection, _Options.MongoDb.Database })
                .AddOdinTransientWithParamasInject<IOdinRedisCache>(ass, new Object[] { _Options.Redis.Connection, _Options.Redis.InstanceName })
                .AddOdinTransientWithParamasInject<IOdinCacheManager>(ass, new Object[] { _Options })
                .AddOdinTransientWithParamasInject<IMvcApiCore>(ass, new Object[] { _Options })
                .AddOdinHttpClient("OdinClient")
                .AddOdinCapInject(new OdinCapEventBusOptions { MysqlConnectionString = _Options.DbEntity.ConnectionString, RabbitmqOptions = _Options.RabbitMQ });
            services.SetServiceProvider();

            // Log.Logger.Information("启用【 数据库配置 】---开始配置");
            SugarIocServices.AddSqlSugar(new IocConfig()
            {
                ConfigId = "1",
                ConnectionString = _Options.DbEntity.ConnectionString,
                DbType = IocDbType.MySql,
                IsAutoCloseConnection = true, //自动释放
            });
            services.ConfigurationSugar(db =>
                {
                    db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices { DataInfoCacheService = services.GetService<IOdinCacheManager>() };
                    //多租户 
                    //db.GetConnection("1").CurrentConnectionConfig.ConfigureExternalServices =xxx
                    //也可以配置AOP
                });
            // services.AddDbContext<OdinProjectEntities>(option =>
            // {
            //     option.UseMySQL(_Options.DbEntity.ConnectionString);
            // });


            #region Log设置
            Log.Logger = new LoggerConfiguration()
                // 最小的日志输出级别
                .MinimumLevel.Information()
                //.MinimumLevel.Information ()
                // 日志调用类命名空间如果以 System 开头，覆盖日志输出最小级别为 Information
                .MinimumLevel.Override("System", LogEventLevel.Information)
                // 日志调用类命名空间如果以 Microsoft 开头，覆盖日志输出最小级别为 Information
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .OdinWriteLog(
                    new LogWriteFileModel { },
                    new LogWriteToConsoleModel { ConsoleTheme = SystemConsoleTheme.Colored },
                    new LogWriteMySqlModel { LogLevels = new int[] { 1, 3, 4, 5 }, ConnectionString = _Options.DbEntity.ConnectionString }
                )
                .CreateLogger();
            #endregion

            Log.Logger.Information("启用【 中文乱码设置 】---开始配置");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            Log.Logger.Information("启用【 AutoMapper自动映射 】---开始配置");
            services.AddAutoMapper(typeof(Program));


            Log.Logger.Information("启用【 HttpClient 依赖注入 】---开始配置");
            var handler = new HttpClientHandler();
            foreach (var cerItem in _Options.SslCers)
            {
                if (!string.IsNullOrEmpty(cerItem.CerPath))
                {
                    var clientCertificate = new X509Certificate2(cerItem.CerPath, cerItem.CerPassword);
                    handler.ClientCertificates.Add(clientCertificate);
                }
            }
            var handlerWithCer = new HttpClientHandler();
            foreach (var cerItem in _Options.SslCers)
            {
                if (!string.IsNullOrEmpty(cerItem.CerPath))
                {
                    var clientCertificate = new X509Certificate2(cerItem.CerPath, cerItem.CerPassword);
                    handlerWithCer.ClientCertificates.Add(clientCertificate);
                }
            }
            services.AddHttpClient("OdinClient", c =>
            {
            }).ConfigurePrimaryHttpMessageHandler(() => handler);
            services.AddHttpClient("OdinClientCer", c =>
            {
            }).ConfigurePrimaryHttpMessageHandler(() => handlerWithCer);

            services.AddHostedService<OdinBackgroundService>();

            services.SetServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
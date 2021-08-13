using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OdinPlugs.OdinHostedService.BgServiceInject;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinInject.InjectPlugs;
using OdinPlugs.OdinInject.InjectPlugs.OdinCacheManagerInject;
using OdinPlugs.OdinWebApi.OdinCore.ConfigModel.Utils;
using OdinPlugs.OdinWebApi.OdinMAF.OdinInject;
using OdinPlugs.OdinWebApi.OdinMAF.OdinSerilog;
using OdinPlugs.OdinWebApi.OdinMAF.OdinSerilog.Models;
using OdinWorkers.Models;
using OdinWorkers.Workers.RabbitMQWorker;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SqlSugar;
using SqlSugar.IOC;
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
            Log.Logger.Information("启用【 强类型配置文件 】");
            services.Configure<ProjectExtendsOptions>(Configuration.GetSection("ProjectConfigOptions"));
            var provider = services.BuildServiceProvider();
            _iOptions = provider.GetRequiredService<IOptionsSnapshot<ProjectExtendsOptions>>();
            _Options = _iOptions.Value;
            services.AddSingleton<ProjectExtendsOptions>(_Options);

            services
                .AddOdinTransientInject(this.GetType().Assembly)
                .AddOdinHttpClient("OdinClient")
                .AddOdinInject(_Options)
                // .AddOdinTransientInject(Assembly.Load("OdinPlugs.ApiLinkMonitor"))
                .AddOdinMapsterTypeAdapter(opt =>
                {
                    // opt.ForType<ErrorCode_DbModel, ErrorCode_Model>()
                    //         .Map(dest => dest.ShowMessage, src => src.CodeShowMessage)
                    //         .Map(dest => dest.ErrorMessage, src => src.CodeErrorMessage);
                })
                .AddOdinTransientInject(Assembly.Load("OdinPlugs.OdinNoSql"))
                ;
            // .AddOdinTransientInject(Assembly.Load("OdinPlugs"));
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
                db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices
                {
                    DataInfoCacheService = services.GetService<IOdinCacheManager>()
                };
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
                .CreateLogger();
            #endregion

            Log.Logger.Information("启用【 中文乱码设置 】---开始配置");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            // 注入 后台服务
            services.AddOdinBgServiceLoopJob(opt =>
                {
                    opt.ActionJob = () =>
                    {
#if DEBUG
                        // Log.Information($"Service:【 BgService - LoopJob - Running 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
#endif
                        new ReceiveRabbitMQHelper().ReceiveMQ(_Options);
                        Thread.Sleep(1000);
                    };
                });
            services.SetServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
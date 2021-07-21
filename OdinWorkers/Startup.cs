using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OdinPlugs.OdinCore.ConfigModel.Utils;
using OdinPlugs.OdinHostedService.BgServiceInject;
using OdinPlugs.OdinInject;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinInject.InjectPlugs;
using OdinPlugs.OdinInject.InjectPlugs.OdinCacheManagerInject;
using OdinPlugs.OdinInject.InjectPlugs.OdinMongoDbInject;
using OdinPlugs.OdinInject.InjectPlugs.OdinRedisInject;
using OdinPlugs.OdinInject.Models.EventBusModels;
using OdinPlugs.OdinInject.Models.RabbitmqModels;
using OdinPlugs.OdinInject.Models.RedisModels;
using OdinPlugs.OdinMAF.OdinInject;
using OdinPlugs.OdinMAF.OdinSerilog;
using OdinPlugs.OdinMAF.OdinSerilog.Models;
using OdinPlugs.OdinMvcCore.MvcCore;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinObject;
using OdinPlugs.SnowFlake.Inject;
using OdinPlugs.SnowFlake.SnowFlakeModel;
using OdinPlugs.SnowFlake.SnowFlakePlugs.ISnowFlake;
using OdinWorkers.Models;
using OdinWorkers.Workers.RabbitMQWorker;
using OdinWorkers.Workers.TestService;
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
                .AddOdinInject(_Options)
                .AddOdinHttpClient("OdinClient")
                // .AddOdinTransientInject(Assembly.Load("OdinPlugs.ApiLinkMonitor"))
                .AddOdinMapsterTypeAdapter(opt =>
                {
                    // opt.ForType<ErrorCode_DbModel, ErrorCode_Model>()
                    //         .Map(dest => dest.ShowMessage, src => src.CodeShowMessage)
                    //         .Map(dest => dest.ErrorMessage, src => src.CodeErrorMessage);
                })
                .AddOdinTransientInject(Assembly.Load("OdinPlugs")); ;
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

            services
                .AddOdinBgServiceJob(opt =>
                {
                    Timer timer = null;
                    void worker(object state)
                    {
#if DEBUG
                        Log.Information($"Service:【 BgService - Running 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
#endif
                    }
                    opt.StartAsyncAction = () =>
                    {
                        timer = new Timer(worker, null, 0, 2000);
                    };
                    opt.ExecuteAsyncAction = () =>
                    {

                    };
                    opt.StopAsyncAction = () =>
                    {
                        timer?.Change(Timeout.Infinite, 0);
                    };
                    opt.DisposeAction = () =>
                    {
                        timer?.Dispose();
                    };
                })
                .AddOdinBgServiceLoopJob(opt =>
                {
                    opt.ActionJob = () =>
                    {
                        // new ReceiveRabbitMQHelper().ReceiveMQ(_Options);
#if DEBUG
                        Log.Information($"Service:【 BgService - LoopJob - Running 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
#endif
                        Thread.Sleep(1000);
                    };
                })
                .AddOdinBgServiceRecurringJob(opt =>
                {
                    opt.Period = TimeSpan.FromSeconds(1);
                    opt.ActionJob = () =>
                    {
                        // new ReceiveRabbitMQHelper().ReceiveMQ(_Options);
#if DEBUG
                        Log.Information($"Service:【 BgService - RecurringJob - Running 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
#endif
                    };
                })
                .AddOdinBgServiceNomalJob(opt =>
                {
                    opt.ActionJob = () =>
                    {
#if DEBUG
                        Log.Information($"Service:【 BgService - Job - Running 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
#endif
                    };
                })
                .AddOdinBgServiceScheduleJob(opt =>
                {
                    opt.DueTime = 5000;
                    opt.ActionJob = () =>
                    {
#if DEBUG
                        Log.Information($"Service:【 BgService - ScheduleJob - Running 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
#endif
                    };
                });
            services.SetServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
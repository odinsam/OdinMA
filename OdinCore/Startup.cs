using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using AspectCore.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Odin.Plugs.OdinCore.ConfigModel;
using Odin.Plugs.OdinCore.Models.ErrorCode;
using Odin.Plugs.OdinExtensions.BasicExtensions.OdinString;
using Odin.Plugs.OdinMAF.OdinCacheManager;
using Odin.Plugs.OdinMAF.OdinCapService;
using Odin.Plugs.OdinMAF.OdinMongoDb;
using Odin.Plugs.OdinMAF.OdinRedis;
using Odin.Plugs.OdinMAF.OdinSerilog;
using Odin.Plugs.OdinMAF.OdinSerilog.Models;
using Odin.Plugs.OdinMiddleware;
using Odin.Plugs.OdinMvcCore.MvcCore;
using Odin.Plugs.OdinMvcCore.OdinFilter;
using Odin.Plugs.OdinMvcCore.OdinInject;
using Odin.Plugs.OdinMvcCore.OdinMiddleware.MiddlewareExtensions;
using Odin.Plugs.OdinNetCore.OdinSnowFlake.SnowFlakeInterface;
using Odin.Plugs.OdinNetCore.OdinSnowFlake.SnowFlakeModel;
using Odin.Plugs.OdinUtils.OdinFiles;
using OdinCore.Models;
using OdinCore.Models.DbModels;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SqlSugar;
using SqlSugar.IOC;

namespace OdinCore
{
#pragma warning disable CS1591
    public class Startup
    {
        private IOptionsSnapshot<ProjectExtendsOptions> _iOptions;
        private ProjectExtendsOptions _Options;
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            EnumEnvironment enumEnvironment = configuration.GetSection("ProjectConfigOptions:EnvironmentName").Value.ToUpper().ToEnum<EnumEnvironment>();
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Add(new JsonConfigurationSource { Path = "serverConfig/cnf.json", Optional = false, ReloadOnChange = true })
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();
            // ~ 按需加载对应项目环境的config
            // ^ 需要注意的是，如果多个配置文件有相同的配置信息，那么后加载的配置文件会覆盖先加载的配置文件(必须是.json格式的配置文件)
            // ~ 按运行环境 加载对应配置文件 
            // ~ 递归serviceConfig文件夹内所有的配置文件 加载及 cnf.config文件 以外的所有配置,
            var rootPath = webHostEnvironment.ContentRootPath + FileHelper.DirectorySeparatorChar; // 获取项目绝对路径
            ConfigLoadHelper.LoadConfigs(enumEnvironment.ToString().ToLower(), Path.Combine(Directory.GetCurrentDirectory(), "serverConfig"), config, rootPath);
            Configuration = config.Build();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Assembly ass = Assembly.Load("Odin.Plugs");

            // Log.Information("启用【 强类型配置文件 】");
            services.Configure<ProjectExtendsOptions>(Configuration.GetSection("ProjectConfigOptions"));
            _iOptions = services.GetService<IOptionsSnapshot<ProjectExtendsOptions>>();
            _Options = _iOptions.Value;
            services.AddSingleton<ConfigOptions>(_Options);

            services.AddOdinSingletonWithParamasInject<IOdinSnowFlake, OdinSnowFlakeOption>(
                ass,
                opt =>
                {
                    opt.DatacenterId = _Options.FrameworkConfig.SnowFlake.DataCenterId;
                    opt.WorkerId = _Options.FrameworkConfig.SnowFlake.WorkerId;
                });

            services.AddOdinTransientInject(this.GetType().Assembly)
                .AddOdinTransientInject(ass)
                .AddOdinTransientWithParamasInject<IOdinMongo>(
                    ass, new Object[] { _Options.MongoDb.MongoConnection, _Options.MongoDb.Database })
                .AddOdinTransientWithParamasInject<IOdinRedisCache>(
                    ass, new Object[] { _Options.Redis.Connection, _Options.Redis.InstanceName })
                .AddOdinTransientWithParamasInject<IOdinCacheManager>(
                    ass, new Object[] { _Options })
                .AddOdinTransientWithParamasInject<IMvcApiCore>(
                    ass, new Object[] { _Options })
                .AddOdinHttpClient("OdinClient")
                .AddOdinCapInject(
                    new OdinCapEventBusOptions
                    {
                        MysqlConnectionString = _Options.DbEntity.ConnectionString,
                        RabbitmqOptions = _Options.RabbitMQ
                    });
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

            #region 初始化数据库
            //修改cnf.config Host配置的链接字符串  enable修改为true，即可自动化初识数据库
            if (_Options.DbEntity.InitDb)
            {
                new OdinProjectSugarDbContext().CreateTable("db_odinCore", false);
            }
            #endregion
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

            Log.Logger.Information("启用【 Ocelot 】---开始配置");
            var ocelotBuilder = services.AddOcelot(Configuration);
            if (_Options.Consul.Enable)
            {
                ocelotBuilder.AddConsul().AddPolly();
            }

            Log.Logger.Information("启用【 中文乱码设置 】---开始配置");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            Log.Logger.Information("启用【 AutoMapper自动映射 】---开始配置");
            services.AddAutoMapper(typeof(Startup));

            Log.Logger.Information("启用【 AspectCore 全局注入 】---开始配置");
            services.ConfigureDynamicProxy(config =>
            {
                // ~ 类型数注入
                // config.Interceptors.AddTyped<type>();

                // ~ 带参数注入
                // config.Interceptors.AddTyped<type>(params);

                // ~ App1命名空间下的Service不会被代理
                // config.NonAspectPredicates.AddNamespace("App1");

                // ~ 最后一级为App1的命名空间下的Service不会被代理
                // config.NonAspectPredicates.AddNamespace("*.App1");

                // ~ ICustomService接口不会被代理
                // config.NonAspectPredicates.AddService("ICustomService");

                // ~ 后缀为Service的接口和类不会被代理
                // config.NonAspectPredicates.AddService("*Service");

                // ~ 命名为Query的方法不会被代理
                // config.NonAspectPredicates.AddMethod("Query");

                // ~ 后缀为Query的方法不会被代理
                // config.NonAspectPredicates.AddMethod("*Query");

                // ~ 带有Service后缀的类的全局拦截器
                // config.Interceptors.AddTyped<CustomInterceptorAttribute>(method => method.Name.EndsWith("MethodName"));

                // ~ 使用通配符的特定全局拦截器
                // config.Interceptors.AddTyped<CustomInterceptorAttribute>(Predicates.ForService("*Service"));
            });

            Log.Logger.Information("启用【 AspectCore 依赖注入 和 代理注册 】---开始配置");
            // ! OdinAspectCoreInterceptorAttribute 需要继承  AbstractInterceptorAttribute
            // services.AddTransient<OdinAspectCoreInterceptorAttribute>().ConfigureDynamicProxy();

            Log.Logger.Information("启用【 跨域配置 】---开始配置");
            string withOrigins = _Options.CrossDomain.AllowOrigin.WithOrigins;
            string policyName = _Options.CrossDomain.AllowOrigin.PolicyName;
            services.AddCors(opts =>
            {
                opts.AddPolicy(policyName, policy =>
                {
                    policy.WithOrigins(withOrigins.Split(','))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            Log.Logger.Information("启用【 版本控制 】---开始配置");
            services.AddApiVersioning(option =>
            {
                //当设置为 true 时, API 将返回响应标头中支持的版本信息。
                option.ReportApiVersions = true;
                //此选项将用于不提供版本的请求。默认情况下, 假定的 API 版本为1.0。
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(_Options.ApiVersion.MajorVersion, _Options.ApiVersion.MinorVersion);
                // option.ApiVersionReader = ApiVersionReader.Combine(
                //         new QueryStringApiVersionReader(),
                //         new HeaderApiVersionReader()
                //         {
                //             HeaderNames = { "apiVersion" }
                //         });
            }).AddResponseCompression();

            Log.Logger.Information("启用【 真实Ip获取 】---开始配置");
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            Log.Logger.Information("启用【 mvc框架 】---开始配置 【  1.添加自定义过滤器\t2.controller返回json大小写控制 默认大小写 】 ");
            services.AddControllers(opt =>
            {
                // opt.Filters.Add<HttpGlobalExceptionFilter>();
                opt.Filters.Add<OdinModelValidationFilter>(1);
                opt.Filters.Add<ApiInvokerFilterAttribute>(2);
                opt.Filters.Add<ApiInvokerResultFilter>();
            })
                .AddNewtonsoftJson(opt =>
                {
                    // 原样输出，后台属性怎么写的，返回的 json 就是怎样的
                    opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    // 驼峰命名法，首字母小写
                    // opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    // 自定义扩展，属性全为小写
                    // opt.SerializerSettings.ContractResolver = new Odin.Plugs.Models.JsonExtends.ToLowerPropertyNamesContractResolver();
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .ConfigureApiBehaviorOptions(o =>
                {
                    // 关闭框架自带的模型验证
                    o.SuppressModelStateInvalidFilter = true;
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v1", Version = "v1.0" });
                options.SwaggerDoc("v2.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v2", Version = "v2.0" });
                // options.SwaggerDoc("LinkTrack-v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v2", Version = "v1" });
                // options.SwaggerDoc("LinkTrack-v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v2", Version = "v2" });
                // options.SwaggerDoc("Test-v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v2", Version = "v1" });
                // options.SwaggerDoc("Test-v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v2", Version = "v2" });
                // options.SwaggerDoc("WeatherForecast-v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v2", Version = "v1" });
                // options.SwaggerDoc("WeatherForecast-v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v2", Version = "v2" });
                // options.SwaggerDoc("Orbit-v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v2", Version = "v1" });
                // options.SwaggerDoc("Orbit-v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v2", Version = "v2" });
                options.AddServer(new OpenApiServer()
                {
                    Url = "",
                    Description = "vvv"
                });
                options.CustomOperationIds(apiDesc =>
                {
                    var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                    return controllerAction.ControllerName + "-" + controllerAction.ActionName;
                });
                // options.DescribeAllParametersInCamelCase();

                options.UseOneOfForPolymorphism();
                //记得设置工程属性:生成xml文档
                var xmlPath = Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml");
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath, true);
                };
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            services.SetServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IOptionsSnapshot<ProjectExtendsOptions> _iOptions, IActionDescriptorCollectionProvider actionProvider, IMapper mapper)
        {

            var options = _iOptions.Value;
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }
            // else
            app.UseOdinAop();
            app.UseStaticFiles();

            app.UseSwagger();

            app.UseHttpsRedirection();

            loggerFactory.AddSerilog();


            app.UseRouting();

            app.UseCors(options.CrossDomain.AllowOrigin.PolicyName);

            //app.UseOdinRequestParams();

            app.UseAuthorization();

            // app.UseKnife4UI(c =>
            // {
            //     c.RoutePrefix = ""; // serve the UI at root
            //     c.SwaggerEndpoint("/v1/api-docs", "V1 Docs");
            //     c.SwaggerEndpoint("/v2/api-docs", "V2 Docs");
            // });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger"; // serve the UI at root
                c.SwaggerEndpoint("/v1.0/api-docs", "v1");
                c.SwaggerEndpoint("/v2.0/api-docs", "v2");

                // c.SwaggerEndpoint("/LinkTrack-v1/api-docs", "LinkTrack-v1");
                // c.SwaggerEndpoint("/LinkTrack-v2/api-docs", "LinkTrack-v2");

                // c.SwaggerEndpoint("/Test-v1/api-docs", "v1");
                // c.SwaggerEndpoint("/Test-v2/api-docs", "v2");

                // c.SwaggerEndpoint("/WeatherForecast-v1/api-docs", "v1");
                // c.SwaggerEndpoint("/WeatherForecast-v2/api-docs", "v2");

                // c.SwaggerEndpoint("/Orbit-v1/api-docs", "v1");
                // c.SwaggerEndpoint("/Orbit-v2/api-docs", "v2");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSwagger("{documentName}/api-docs");
            });

            Program.ApiComments = new OdinApiCommentCore(options).GetApiComments(actionProvider);

            InitErrorCode(mapper);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        public void InitErrorCode(IMapper mapper)
        {
            var errorCodelst = mapper.Map<List<ErrorCode_Model>>(DbScoped.Sugar.Queryable<ErrorCode_DbModel>().ToList());
            var cacheManager = OdinInjectHelper.GetService<IOdinCacheManager>();
            foreach (var item in errorCodelst)
            {
                cacheManager.Cover(item.ErrorCode, item);
            }
        }
    }
#pragma warning restore CS1591
}
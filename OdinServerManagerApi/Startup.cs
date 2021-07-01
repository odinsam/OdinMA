using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using AspectCore.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
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
using OdinPlugs.ConfigModel;
using OdinPlugs.Files;
using OdinPlugs.OdinCore;
using OdinPlugs.OdinFilter;
using OdinPlugs.OdinMongo;
using OdinPlugs.OdinString;
using OdinPlugs.OdinInject;
using OdinPlugs.WebApi;
using Serilog;
using Serilog.Events;
using OdinServerManagerApi.Models;
using OdinServerManagerApi.Models.DbModels;
using OdinPlugs.OdinSerilog;
using OdinPlugs.OdinSerilog.Models;
using OdinPlugs.OdinServices;
using OdinPlugs.OdinCacheManager;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace OdinServerManagerApi
{
    public class Startup
    {
        private IOptionsSnapshot<ProjectExtendsOptions> _iOptions;
        private ProjectExtendsOptions _Options;
        public IConfiguration Configuration { get; }

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
            var rootPath = webHostEnvironment.ContentRootPath + FileHelper.DirectorySeparatorChar;  // 获取项目绝对路径
            ConfigLoadHelper.LoadConfigs(enumEnvironment.ToString().ToLower(), Path.Combine(Directory.GetCurrentDirectory(), "serverConfig"), config, rootPath);
            Configuration = config.Build();

            #region Log设置
            string mysqlConnectionString = string.Empty;
            if (Convert.ToBoolean(Configuration.GetSection("ProjectConfigOptions:FrameworkConfig:Serilog").Value))
                mysqlConnectionString = Configuration.GetSection("ProjectConfigOptions:DbEntity:ConnectionString").Value;
            Log.Logger = new LoggerConfiguration()
                            // 最小的日志输出级别
                            .MinimumLevel.Information()
                            //.MinimumLevel.Information ()
                            // 日志调用类命名空间如果以 System 开头，覆盖日志输出最小级别为 Information
                            .MinimumLevel.Override("System", LogEventLevel.Information)
                            // 日志调用类命名空间如果以 Microsoft 开头，覆盖日志输出最小级别为 Information
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .OdinWriteLog(
                                new LogWriteFileModel { }, new LogWriteToConsoleModel { }, new LogWriteMySqlModel { ConnectionString = mysqlConnectionString, LogLevels = new int[] { 1, 4, 5 } }
                            )
                            .CreateLogger();
            #endregion

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger.Information("启用【 强类型配置文件 】");
            services.Configure<ProjectExtendsOptions>(Configuration.GetSection("ProjectConfigOptions"));
            var provider = services.BuildServiceProvider();
            _iOptions = provider.GetRequiredService<IOptionsSnapshot<ProjectExtendsOptions>>();
            _Options = _iOptions.Value;

            Log.Logger.Information("启用【 数据库配置 】---开始配置");
            services.AddDbContext<OdinIdentityEntities>(option =>
            {
                option.UseMySQL(_Options.DbEntity.ConnectionString);
            });


            Log.Logger.Information("启用【 mvc框架 】---开始配置 【  1.添加自定义过滤器\t2.controller返回json大小写控制 默认大小写 】 ");
            services.AddControllers(opt =>
               {
                   opt.Filters.Add(new HttpGlobalExceptionFilter(_Options));
                   opt.Filters.Add(new ApiInvokerFilterAttribute(_Options));
                   // opt.Filters.Add(new AuthenFilterAttribute(_apiCnfOptions)); //自定义token全局拦截器
               })
               .AddNewtonsoftJson(opt =>
               {
                   // 原样输出，后台属性怎么写的，返回的 json 就是怎样的
                   opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                   // 驼峰命名法，首字母小写
                   // opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                   // 自定义扩展，属性全为小写
                   // opt.SerializerSettings.ContractResolver = new OdinPlugs.Models.JsonExtends.ToLowerPropertyNamesContractResolver();
               }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);





            if (_Options.FrameworkConfig.Ocelot.Enable)
            {
                Log.Logger.Information("启用【 Ocelot 】---开始配置");
                var ocelotBuilder = services.AddOcelot(Configuration);
                if (_Options.Consul.Enable)
                {
                    ocelotBuilder.AddConsul().AddPolly();
                }
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



            Log.Logger.Information("启用【  当前项目以及DLL 带参数 依赖注入 】---开始配置");
            // ! services.AddOdinSingletonServicesWithParams<IService>(this.GetType().Assembly, new Object[] { dbContext, new MvcApiCore(_Options) });



            if (_Options.FrameworkConfig.AspectCore.Enable)
            {
                Log.Logger.Information("启用【 AspectCore 依赖注入 和 代理注册 】---开始配置");
                // ! OdinAspectCoreInterceptorAttribute 需要继承  AbstractInterceptorAttribute
                // services.AddTransient<OdinAspectCoreInterceptorAttribute>().ConfigureDynamicProxy();
            }

            if (_Options.CrossDomain.AllowOrigin.Enable)
            {
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
            }



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




            Log.Logger.Information("启用【 版本控制 】---开始配置");
            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(
                    _Options.ApiVersion.MajorVersion,
                    _Options.ApiVersion.MinorVersion);
            }).AddResponseCompression();



            Assembly ass = Assembly.Load("OdinPlugs");
            services
                    .AddOdinSingletonInject(this.GetType().Assembly)
                    .AddOdinSingletonInject(ass)
                    .AddOdinSingletonWithParamasInject<IMongoHelper>(ass, new Object[] { _Options.MongoDb.MongoConnection, _Options.MongoDb.Database })
                    .AddOdinSingletonWithParamasInject<IOdinCacheManager>(ass, new Object[] { _Options })
                    .AddOdinSingletonWithParamasInject<IMvcApiCore>(ass, new Object[] { _Options });


            OdinInjectHelper.ServiceProvider = services.BuildServiceProvider();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IOptionsSnapshot<ProjectExtendsOptions> _iOptions, OdinIdentityEntities entity, IActionDescriptorCollectionProvider actionProvider, IMapper mapper)
        {
            var options = _iOptions.Value;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            loggerFactory.AddSerilog();
            if (options.CrossDomain.AllowOrigin.Enable)
            {
                app.UseCors(options.CrossDomain.AllowOrigin.PolicyName);
            }

            app.UseRouting();

            app.UseAuthorization();

            #region 初始化数据库
            //修改cnf.config Host配置的链接字符串  enable修改为true，即可自动化初识数据库
            if (options.DbEntity.InitDb)
            {
                var flag = entity.Database.EnsureCreated();
                Log.Logger.Information($"自动创建数据库:{flag}");
                if (flag)
                {
                    Log.Logger.Information($"启用【 数据库初始化 】---开始配置");
                    SampleData.Init(entity);
                }
            }
            #endregion

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

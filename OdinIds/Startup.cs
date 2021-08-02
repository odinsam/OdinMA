using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using OdinIds.Models;
using OdinIds.Models.DbModels;
using OdinPlugs.ApiLinkMonitor.MiddlewareExtensions;
using OdinPlugs.ApiLinkMonitor.OdinAspectCore.IOdinAspectCoreInterface;
using OdinPlugs.ApiLinkMonitor.OdinMiddleware.MiddlewareExtensions;
using OdinPlugs.OdinCore.ConfigModel;
using OdinPlugs.OdinCore.ConfigModel.Utils;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinInject.InjectPlugs;
using OdinPlugs.OdinMAF.OdinId4Services.OdinId4Extensions;
using OdinPlugs.OdinMAF.OdinInject;
using OdinPlugs.OdinMAF.OdinSerilog;
using OdinPlugs.OdinMAF.OdinSerilog.Models;
using OdinPlugs.OdinMvcCore.MvcCore;
using OdinPlugs.OdinMvcCore.OdinExtensions;
using OdinPlugs.OdinMvcCore.OdinFilter;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinString;
using OdinPlugs.OdinUtils.Utils.OdinFiles;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace OdinIds
{
    public class Startup
    {
        private IOptionsSnapshot<ProjectExtendsOptions> _iOptions;
        private ProjectExtendsOptions _Options;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            var enumEnvironment = configuration.GetSection("ProjectConfigOptions:EnvironmentName").Value.ToUpper().ToEnum<EnumEnvironment>();
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
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger.Information("启用【 强类型配置文件 】");
            services.Configure<ProjectExtendsOptions>(Configuration.GetSection("ProjectConfigOptions"));
            _iOptions = services.GetRegisteredRequiredService<IOptionsSnapshot<ProjectExtendsOptions>>();
            _Options = _iOptions.Value;

            services
                .AddOdinTransientInject(this.GetType().Assembly)
                .AddOdinTransientInject(Assembly.Load("OdinPlugs.ApiLinkMonitor"))
                .AddOdinInject(_Options)
                .AddOdinHttpClient("OdinClient")
                // .AddOdinTransientInject(Assembly.Load("OdinPlugs.ApiLinkMonitor"))
                .AddOdinMapsterTypeAdapter(opt =>
                {
                    // opt.ForType<ErrorCode_DbModel, ErrorCode_Model>()
                    //         .Map(dest => dest.ShowMessage, src => src.CodeShowMessage)
                    //         .Map(dest => dest.ErrorMessage, src => src.CodeErrorMessage);
                })
                .AddOdinTransientInject(Assembly.Load("OdinPlugs"));
            // services.AddSingleton<IOdinSnowFlake>(provider => new OdinSnowFlake(1, 1));
            // services.AddTransient<OdinAspectCoreInterceptorAttribute>().ConfigureDynamicProxy();
            services.SetServiceProvider();

            Log.Logger.Information("启用【 数据库配置 】---开始配置");
            services.AddDbContext<OdinIdentityEntities>(option =>
            {
                option.UseMySQL(_Options.DbEntity.ConnectionString);
            });

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
                    new LogWriteToConsoleModel { ConsoleTheme = SystemConsoleTheme.Colored }
                    // new LogWriteMySqlModel { LogLevels = new int[] { 1, 3, 4, 5 }, ConnectionString = _Options.DbEntity.ConnectionString }
                )
                .CreateLogger();
            #endregion

            Log.Logger.Information("启用【 中文乱码设置 】---开始配置");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

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
                opt.Filters.Add<HttpGlobalExceptionFilter>();
                opt.Filters.Add<OdinModelValidationFilter>(1);
                opt.Filters.Add<ApiInvokerFilterAttribute>(2);
                opt.Filters.Add<ApiInvokerResultFilter>();
            })
                .AddNewtonsoftJson(opt =>
                {
                    var contractResolver = new DefaultContractResolver();
                    // contractResolver.ResolveContract(typeof(JsonConverterLongContractResolver));
                    // opt.SerializerSettings.ContractResolver = contractResolver;
                    // 原样输出，后台属性怎么写的，返回的 json 就是怎样的
                    // opt.SerializerSettings.ContractResolver = new JsonConverterLongContractResolver();
                    // opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    // 驼峰命名法，首字母小写
                    // opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    // 自定义扩展，属性全为小写
                    // opt.SerializerSettings.ContractResolver = new OdinPlugs.Models.JsonExtends.ToLowerPropertyNamesContractResolver();
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .ConfigureApiBehaviorOptions(o =>
                {
                    // 关闭框架自带的模型验证
                    o.SuppressModelStateInvalidFilter = true;
                });

            Log.Logger.Information("启用【 AspectCore 全局注入 】---开始配置");
            services.ConfigureDynamicProxy(config =>
            {
                // config.Interceptors.AddServiced<FoobarAttribute>();
                // ~ 类型数注入

                // config.Interceptors.AddTyped<FoobarAttribute>();

                // ~ 带参数注入
                // config.Interceptors.AddTyped<OdinAspectCoreInterceptorAttribute>(new Object[] { "d" }, new AspectPredicate[] { });

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
                config.Interceptors.AddTyped<OdinAspectCoreInterceptorAttribute>(Predicates.ForService("*Service"));
            });

            Log.Logger.Information("启用【 AspectCore 依赖注入 和 代理注册 】---开始配置");
            // ! OdinAspectCoreInterceptorAttribute 需要继承  AbstractInterceptorAttribute
            // services.AddTransient<OdinAspectCoreInterceptorAttribute>().ConfigureDynamicProxy();

            services.AddDbContext<OdinIdentityEntities>(opt =>
            {
                opt.UseMySQL(_Options.DbEntity.ConnectionString);
            });

            // 启用 Identity 服务 添加指定的用户和角色类型的默认标识系统配置
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var ids = services.AddIdentityServer();
            if (!File.Exists("rsaCers/odin_ids.rsa"))
            {
                Log.Logger.Information("新建 ids 秘钥文件");
                ids = ids.AddDeveloperSigningCredential(true, "rsaCers/odin_ids.rsa");
            }
            else
            {
                Log.Logger.Information("ids 秘钥文件已存在。");
                ids = ids.AddDeveloperSigningCredential(filename: "rsaCers/odin_ids.rsa");
            }
            // 客户端和资源的数据库存储
            // ConfigurationDbContext
            // dotnet ef migrations add ConfigDbContext -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfiguragtionDb
            // dotnet ef database update -c ConfigurationDbContext
            ids.AddConfigurationStore(opt =>
            {
                opt.ConfigureDbContext = context =>
                {
                    context.UseMySQL(_Options.DbEntity.ConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                };
            })
                // 令牌和授权码的数据库存储
                // PersistedGrantDbContext
                // dotnet ef migrations add OperationContext -c PersistedGrantDbContext  -o Data/Migrations/IdentityServer/OperationDb
                // dotnet ef database update -c PersistedGrantDbContext
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = context =>
                        context.UseMySQL(_Options.DbEntity.ConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    opt.EnableTokenCleanup = true;
                    opt.TokenCleanupInterval = 30;
                });

            services.AddIdentityServerDbContext<ConfigurationDbContext>(options =>
            {
                options.ConfigureDbContext = builder => builder.UseMySQL(_Options.DbEntity.ConnectionString, db => db.MigrationsAssembly(migrationsAssembly));
            })
                .AddIdentityServerDbContext<PersistedGrantDbContext>(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseMySQL(_Options.DbEntity.ConnectionString, db => db.MigrationsAssembly(migrationsAssembly));
                });

            // 更改Identity中关于用户和角色的处理到Entityframework
            // dotnet ef migrations add UserStoreContext -c OdinIdentityEntities -o Data/Migrations/IdentityServer/UserDb
            // dotnet ef database update -c OdinIdentityEntities

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "odinIds", Version = "v1.0" });
                options.AddServer(new OpenApiServer()
                {
                    Url = "http://0.0.0.0:20505/swagger/index.html",
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
                //添加Authorization
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string> ()
                    }
                });
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
            services.SetServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, OdinIdentityEntities entity,
            IHttpContextAccessor svp)
        {
            MvcContext.httpContextAccessor = svp;
            var options = _iOptions.Value;

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseOdinApiLinkMonitor(
                // 添加需要过滤 无需链路监控的RequestPath
                opts =>
                {
                    opts.Add(@"\/knife4j");
                }
            );
            app.UseOdinException();
            // app.UseIdentityServer();

            app.UseSwagger();

            app.UseHttpsRedirection();

            loggerFactory.AddSerilog();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger"; // serve the UI at root
                c.SwaggerEndpoint("/v1.0/api-docs", "v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSwagger("{documentName}/api-docs");
            });

            #region 初始化数据库
            InitializeDatabase(app);
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
            // 
            #endregion
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                var userContext = serviceScope.ServiceProvider.GetRequiredService<OdinIdentityEntities>();
                //添加config中的客户端数据到数据库
                if (!context.Clients.Any())
                {
                    foreach (var client in IdentityConfig.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                }
                //添加config中的IdentityResources数据到数据库
                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in IdentityConfig.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                }
                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in IdentityConfig.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                }

                if (!userContext.IdentityUsers.Any())
                {
                    foreach (var user in IdentityConfig.GetUsers())
                    {
                        // System.Console.WriteLine(JsonConvert.SerializeObject(user).ToJsonFormatString());
                        userContext.IdentityUsers.Add(user);
                    }
                }
                if (!userContext.Stus.Any())
                {
                    foreach (var stu in IdentityConfig.GetStus())
                    {
                        // System.Console.WriteLine(JsonConvert.SerializeObject(user).ToJsonFormatString());
                        userContext.Stus.Add(stu);
                    }
                }
                //添加config中的ApiResources数据到数据库
                // if (!context.ApiResources.Any())
                // {
                //     foreach (var resource in IdentityConfig.GetApis())
                //     {
                //         context.ApiResources.Add(resource.ToEntity());
                //     }
                // }
                userContext.SaveChanges();
                context.SaveChanges();
            }
        }
    }
}
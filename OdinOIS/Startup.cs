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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Odin.Plugs.ConfigModel;
using Odin.Plugs.Files;
using Odin.Plugs.OdinFilter;
using Odin.Plugs.OdinMongo;
using Odin.Plugs.OdinString;
using Odin.Plugs.OdinInject;
using Odin.Plugs.WebApi;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Odin.Plugs.OdinCore;
using Newtonsoft.Json;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.IdentityModel.Tokens;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Interfaces;
using Odin.Plugs.OdinServices;
using Odin.Plugs.OdinServices.IdentityServer;
using OdinOIS.Models.DbModels;
using OdinOIS.Models;
using Serilog.Events;
using Odin.Plugs.OdinSerilog;
using Odin.Plugs.OdinSerilog.Models;
using Odin.Plugs.WebApi.HttpClientHelper;
using Odin.Plugs.OdinRedis;
using Odin.Plugs.OdinCacheManager;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace OdinOIS
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
            Log.Logger = new LoggerConfiguration()
                // 最小的日志输出级别
                .MinimumLevel.Information()
                //.MinimumLevel.Information ()
                // 日志调用类命名空间如果以 System 开头，覆盖日志输出最小级别为 Information
                .MinimumLevel.Override("System", LogEventLevel.Information)
                // 日志调用类命名空间如果以 Microsoft 开头，覆盖日志输出最小级别为 Information
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .OdinWriteLog(
                    new LogWriteFileModel { }, new LogWriteToConsoleModel { }, new LogWriteMySqlModel { ConnectionString = Configuration.GetSection("ProjectConfigOptions:DbEntity:ConnectionString").Value }
                )
                .CreateLogger();
            #endregion
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger.Information("启用【 强类型配置文件 】");
            services.Configure<ProjectExtendsOptions>(Configuration.GetSection("ProjectConfigOptions"));
            _iOptions = services.GetRegisteredRequiredService<IOptionsSnapshot<ProjectExtendsOptions>>();
            _Options = _iOptions.Value;


            Log.Logger.Information("启用【 数据库配置 】---开始配置");
            services.AddDbContext<OdinIdentityEntities>(option =>
            {
                option.UseMySQL(_Options.DbEntity.ConnectionString);
            });



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


            if (_Options.FrameworkConfig.AspectCore.Enable)
            {
                Log.Logger.Information("启用【 AspectCore 依赖注入 和 代理注册 】---开始配置");
                // ! OdinAspectCoreInterceptorAttribute 需要继承  AbstractInterceptorAttribute
                // services.AddTransient<OdinAspectCoreInterceptorAttribute>().ConfigureDynamicProxy();
            }

            if (_Options.IdentityServer.Enable)
            {
                // 启用 Identity 服务 添加指定的用户和角色类型的默认标识系统配置
                var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
                services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    // 客户端和资源的数据库存储
                    // ConfigurationDbContext
                    // dotnet ef migrations add ConfigDbContext -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfiguragtionDb
                    // dotnet ef database update -c ConfigurationDbContext
                    .AddConfigurationStore(opt =>
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
            }


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
                   // opt.SerializerSettings.ContractResolver = new Odin.Plugs.Models.JsonExtends.ToLowerPropertyNamesContractResolver();
               }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


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



            Assembly ass = Assembly.Load("Odin.Plugs");
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

            app.UseIdentityServer();

            app.UseHttpsRedirection();

            loggerFactory.AddSerilog();

            app.UseRouting();


            if (options.CrossDomain.AllowOrigin.Enable)
            {
                app.UseCors(options.CrossDomain.AllowOrigin.PolicyName);
            }

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

            if (options.IdentityServer.Enable)
            {
                app.UseAuthorization();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
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
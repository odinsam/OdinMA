using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using IGeekFan.AspNetCore.Knife4jUI;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using OdinCore.Models;
using OdinCore.Models.DbModels;
using OdinPlugs.ApiLinkMonitor.MiddlewareExtensions;
using OdinPlugs.ApiLinkMonitor.OdinAspectCore.IOdinAspectCoreInterface;
using OdinPlugs.ApiLinkMonitor.OdinMiddleware.MiddlewareExtensions;
using OdinPlugs.OdinCore.ConfigModel;
using OdinPlugs.OdinCore.ConfigModel.Utils;
using OdinPlugs.OdinCore.Models.ErrorCode;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinInject.InjectPlugs;
using OdinPlugs.OdinInject.InjectPlugs.OdinCacheManagerInject;
using OdinPlugs.OdinInject.InjectPlugs.OdinMapsterInject;
using OdinPlugs.OdinInject.Models.CacheManagerModels;
using OdinPlugs.OdinInject.Models.RabbitmqModels;
using OdinPlugs.OdinInject.Models.RedisModels;
using OdinPlugs.OdinMAF.OdinInject;
using OdinPlugs.OdinMAF.OdinSerilog;
using OdinPlugs.OdinMAF.OdinSerilog.Models;
using OdinPlugs.OdinMvcCore.MvcCore;
using OdinPlugs.OdinMvcCore.OdinExtensions;
using OdinPlugs.OdinMvcCore.OdinFilter;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinAdapterMapper;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinString;
using OdinPlugs.OdinUtils.Utils.OdinFiles;
using OdinPlugs.SnowFlake.Inject;
using OdinPlugs.SnowFlake.SnowFlakeModel;
using OdinPlugs.SnowFlake.SnowFlakePlugs.ISnowFlake;
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
            // ~ ?????????????????????????????????config
            // ^ ???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????(?????????.json?????????????????????)
            // ~ ??????????????? ???????????????????????? 
            // ~ ??????serviceConfig????????????????????????????????? ????????? cnf.config?????? ?????????????????????,
            var rootPath = webHostEnvironment.ContentRootPath + FileHelper.DirectorySeparatorChar; // ????????????????????????
            ConfigLoadHelper.LoadConfigs(enumEnvironment.ToString().ToLower(), Path.Combine(Directory.GetCurrentDirectory(), "serverConfig"), config, rootPath);
            Configuration = config.Build();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Log.Information("????????? ????????????????????? ???");
            services.Configure<ProjectExtendsOptions>(Configuration.GetSection("ProjectConfigOptions"));
            services.SetServiceProvider();
            _iOptions = services.GetService<IOptionsSnapshot<ProjectExtendsOptions>>();
            _Options = _iOptions.Value;
            services.AddSingleton<ConfigOptions>(_Options);

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

            // Log.Logger.Information("????????? ??????????????? ???---????????????");
            SugarIocServices.AddSqlSugar(new IocConfig()
            {
                ConfigId = "1",
                ConnectionString = _Options.DbEntity.ConnectionString,
                DbType = IocDbType.MySql,
                IsAutoCloseConnection = true, //????????????
            });
            services.ConfigurationSugar(db =>
            {
                db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices
                {
                    DataInfoCacheService = services.GetService<IOdinCacheManager>()
                };
                //????????? 
                //db.GetConnection("1").CurrentConnectionConfig.ConfigureExternalServices =xxx
                //???????????????AOP
            });

            #region ??????????????????
            //??????cnf.config Host????????????????????????  enable?????????true?????????????????????????????????
            if (_Options.DbEntity.InitDb)
            {
                new OdinProjectSugarDbContext().CreateTable("db_odinCore", false);
            }
            #endregion
            // services.AddDbContext<OdinProjectEntities>(option =>
            // {
            //     option.UseMySQL(_Options.DbEntity.ConnectionString);
            // });

            #region Log??????
            Log.Logger = new LoggerConfiguration()
                // ???????????????????????????
                .MinimumLevel.Information()
                //.MinimumLevel.Information ()
                // ???????????????????????????????????? System ?????????????????????????????????????????? Information
                .MinimumLevel.Override("System", LogEventLevel.Information)
                // ???????????????????????????????????? Microsoft ?????????????????????????????????????????? Information
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .OdinWriteLog(
                    new LogWriteFileModel { },
                    new LogWriteToConsoleModel { ConsoleTheme = SystemConsoleTheme.Colored }
                    // new LogWriteMySqlModel { LogLevels = new int[] { 1, 3, 4, 5 }, ConnectionString = _Options.DbEntity.ConnectionString }
                )
                .CreateLogger();
            #endregion

            Log.Logger.Information("????????? Ocelot ???---????????????");
            var ocelotBuilder = services.AddOcelot(Configuration);
            if (_Options.Consul.Enable)
            {
                ocelotBuilder.AddConsul().AddPolly();
            }

            Log.Logger.Information("????????? ?????????????????? ???---????????????");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            Log.Logger.Information("????????? ???????????? ???---????????????");
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

            Log.Logger.Information("????????? ???????????? ???---????????????");
            services.AddApiVersioning(option =>
            {
                //???????????? true ???, API ????????????????????????????????????????????????
                option.ReportApiVersions = true;
                //????????????????????????????????????????????????????????????, ????????? API ?????????1.0???
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(_Options.ApiVersion.MajorVersion, _Options.ApiVersion.MinorVersion);
                // option.ApiVersionReader = ApiVersionReader.Combine(
                //         new QueryStringApiVersionReader(),
                //         new HeaderApiVersionReader()
                //         {
                //             HeaderNames = { "apiVersion" }
                //         });
            }).AddResponseCompression();

            Log.Logger.Information("????????? ??????Ip?????? ???---????????????");
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            Log.Logger.Information("????????? mvc?????? ???---???????????? ???  1.????????????????????????\t2.controller??????json??????????????? ??????????????? ??? ");
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
                    // ??????????????????????????????????????????????????? json ???????????????
                    // opt.SerializerSettings.ContractResolver = new JsonConverterLongContractResolver();
                    // opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    // ?????????????????????????????????
                    // opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    // ????????????????????????????????????
                    // opt.SerializerSettings.ContractResolver = new OdinPlugs.Models.JsonExtends.ToLowerPropertyNamesContractResolver();
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .ConfigureApiBehaviorOptions(o =>
                {
                    // ?????????????????????????????????
                    o.SuppressModelStateInvalidFilter = true;
                });
            // services.AddTransient<ITestService, TestService>();
            // services.SetInterceptableServiceProvider();
            services.ConfigureDynamicProxy(config =>
            {
                // config.Interceptors.AddServiced<FoobarAttribute>();
                // ~ ???????????????

                // config.Interceptors.AddTyped<FoobarAttribute>();

                // ~ ???????????????
                // config.Interceptors.AddTyped<OdinAspectCoreInterceptorAttribute>(new Object[] { "d" }, new AspectPredicate[] { });

                // ~ App1??????????????????Service???????????????
                // config.NonAspectPredicates.AddNamespace("App1");

                // ~ ???????????????App1?????????????????????Service???????????????
                // config.NonAspectPredicates.AddNamespace("*.App1");

                // ~ ICustomService?????????????????????
                // config.NonAspectPredicates.AddService("ICustomService");

                // ~ ?????????Service??????????????????????????????
                // config.NonAspectPredicates.AddService("*Service");

                // ~ ?????????Query????????????????????????
                // config.NonAspectPredicates.AddMethod("Query");

                // ~ ?????????Query????????????????????????
                // config.NonAspectPredicates.AddMethod("*Query");

                // ~ ??????Service??????????????????????????????
                // config.Interceptors.AddTyped<CustomInterceptorAttribute>(method => method.Name.EndsWith("MethodName"));

                // ~ ???????????????????????????????????????
                config.Interceptors.AddTyped<OdinAspectCoreInterceptorAttribute>(Predicates.ForService("*Service"));
            });
            Log.Logger.Information("????????? AspectCore ???????????? ??? ???????????? ???---????????????");
            // ! OdinAspectCoreInterceptorAttribute ????????????  AbstractInterceptorAttribute

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v1", Version = "v1.0" });
                options.SwaggerDoc("v2.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI v2", Version = "v2.0" });
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
                //????????????????????????:??????xml??????
                var xmlPath = Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml");
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath, true);
                };
                //??????Authorization
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
            // services.ConfigureDynamicProxy();
            // services.AddSingleton<FoobarAttribute>();
            // services.ConfigureDynamicProxy(config => { config.Interceptors.AddServiced<FoobarAttribute>(); });

            // services.AddTransient<FoobarAttribute>().ConfigureDynamicProxy();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "http://127.0.0.1:20505";
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });
            services.SetServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IOptionsSnapshot<ProjectExtendsOptions> _iOptions, IActionDescriptorCollectionProvider actionProvider, IHttpContextAccessor svp)
        {
            MvcContext.httpContextAccessor = svp;
            var options = _iOptions.Value;

            app.UseStaticFiles();
            app.UseOdinApiLinkMonitor(
                // ?????????????????? ?????????????????????RequestPath
                opts =>
                {
                    opts.Add(@"\/knife4j");
                }
            );
            app.UseOdinException();

            app.UseSwagger();

            app.UseHttpsRedirection();

            loggerFactory.AddSerilog();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(options.CrossDomain.AllowOrigin.PolicyName);

            app.UseAuthorization();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger"; // serve the UI at root
                c.SwaggerEndpoint("/v1.0/api-docs", "v1");
                c.SwaggerEndpoint("/v2.0/api-docs", "v2");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSwagger("{documentName}/api-docs");
            });

            Program.ApiComments = new OdinApiCommentCore(options).GetApiComments(actionProvider);

            InitErrorCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        public void InitErrorCode()
        {
            var errorCodes = DbScoped.Sugar.Queryable<ErrorCode_DbModel>().ToList();
            var errorCodelst = errorCodes
                .OdinTypeAdapterBuilder<ErrorCode_DbModel, ErrorCode_Model, List<ErrorCode_Model>>(
                    opt =>
                    {
                        opt.Map(dest => dest.Id, src => src.Id.ToString());
                        opt.Map(dest => dest.ErrorMessage, src => src.CodeErrorMessage);
                        opt.Map(dest => dest.ShowMessage, src => src.CodeShowMessage);
                    },
                    OdinInjectCore.GetService<IOdinMapster>().GetConfig()
                );
            var cacheManager = OdinInjectCore.GetService<IOdinCacheManager>();
            foreach (var item in errorCodelst)
            {
                cacheManager.Cover(item.ErrorCode, item);
            }
        }
    }
#pragma warning restore CS1591
}
using System;
using System.Collections.Generic;
using System.IO;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OdinPlugs.OdinCore.Models;
using OdinPlugs.OdinMvcCore.OdinWebHost;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinObject;
using Serilog;
using Unicorn.AspNetCore.Middleware.RealIp;

namespace OdinIds
{
    public class Program
    {
        public static IEnumerable<ApiCommentConfig> ApiComments { get; set; }
        public static void Main(string[] args)
        {
            try
            {
                var odinWebHostManager = OdinWebHostManager.Load();
                do
                {
                    odinWebHostManager.Start<Startup>(CreateHostBuilder(args));
                } while (odinWebHostManager.Restarting);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("服务器启动失败");
                System.Console.WriteLine(ex.ToJson(enumStringFormat.Json));
                // System.Console.WriteLine(JsonConvert.SerializeObject(ex).ToJson(enumStringFormat.Json));
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builderRoot = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();
            if (File.Exists("serverConfig/cnf.json"))
            {
                builderRoot = builderRoot.Add(new JsonConfigurationSource { Path = "serverConfig/cnf.json", Optional = false, ReloadOnChange = true });
            }
            var builder = builderRoot.Build();
            var iHostBuilder = Host.CreateDefaultBuilder(args);

            // iHostBuilder = iHostBuilder.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
            return iHostBuilder.ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(builder)
                    .UseKestrel(
                        (context, options) =>
                        {
                            options.AllowSynchronousIO = true;
                            //设置应用服务器Kestrel请求体最大为200MB
                            options.Limits.MaxRequestBodySize = 209715200;
                        }
                    )
                    .UseIISIntegration()
                    .UseUrls(builder.GetValue<string>("ProjectConfigOptions:Url").Split(','))
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseRealIp("X-Forwarded-For")
                    .UseStartup<Startup>()
                    .UseSerilog();
                }).UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
        }
    }
}

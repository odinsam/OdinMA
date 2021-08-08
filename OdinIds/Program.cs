using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

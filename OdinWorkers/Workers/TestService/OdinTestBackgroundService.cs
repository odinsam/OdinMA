using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinString;
using OdinWorkers.Models;
using Serilog;

namespace OdinWorkers.Workers.TestService
{
    public class OdinTestBackgroundService : BackgroundService
    {
        private Timer _timer1;
        public OdinTestBackgroundService()
        {
        }
        private void DoWork(object state)
        {
            try
            {
                Log.Information($"OdinTestBackgroundService:【 Run 】\tTime:【{DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine(JsonConvert.SerializeObject(ex.Message).ToJsonFormatString());
                System.Console.WriteLine("");
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("后台服务 【 OdinBackgroundService 】 【 running 】");
            return ExecuteAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer1 = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(5000));
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information($"Service:【 Stop 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
            _timer1?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _timer1?.Dispose();
        }
    }
}
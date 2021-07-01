using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OdinPlugs.OdinExtensions.BasicExtensions.OdinString;
using OdinWorkers.Models;
using Serilog;

namespace OdinWorkers.Workers.RabbitMQWorker
{
    public class OdinBackgroundService : BackgroundService
    {
        private readonly ProjectExtendsOptions apiOptions;
        private readonly ReceiveRabbitMQHelper receiveRabbitMQHelper;
        private int executionCount = 0;
        private Timer _timer;
        public OdinBackgroundService(ProjectExtendsOptions options)
        {
            this.apiOptions = options;
            this.receiveRabbitMQHelper = new ReceiveRabbitMQHelper();
        }

        private void DoWork(object state)
        {
            try
            {
                receiveRabbitMQHelper.ReceiveMQ(apiOptions);
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

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000));
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information($"Service:【 Stop 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
            _timer?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
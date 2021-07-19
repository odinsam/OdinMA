using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinString;
using OdinWorkers.Models;
using Serilog;

namespace OdinWorkers.Workers.RabbitMQWorker
{
    public class OdinBackgroundService : BackgroundService
    {
        private readonly ProjectExtendsOptions apiOptions;
        private readonly ReceiveRabbitMQHelper receiveRabbitMQHelper;
        private int executionCount = 0;
        private Timer _timer1;
        private Timer _timer2;
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
        private void DoWork2(object state)
        {
            try
            {
                System.Console.WriteLine("=================do work 2 ===============");
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
            _timer1 = new Timer(DoWork, null, 0, Timeout.Infinite);
            _timer2 = new Timer(DoWork2, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(5000));
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information($"Service:【 Stop 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
            _timer1?.Change(Timeout.Infinite, 0);
            _timer2?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _timer1?.Dispose();
            _timer2?.Dispose();
        }
    }
}
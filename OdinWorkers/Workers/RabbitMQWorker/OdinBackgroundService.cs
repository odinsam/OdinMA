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
        Task worker;
        int i = 10;
        public OdinBackgroundService(ProjectExtendsOptions options)
        {
            this.apiOptions = options;
            this.receiveRabbitMQHelper = new ReceiveRabbitMQHelper();
        }
        private async Task DoWorkAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    int count = Interlocked.Increment(ref executionCount);
                    receiveRabbitMQHelper.ReceiveMQ(apiOptions);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(JsonConvert.SerializeObject(ex.Message).ToJsonFormatString());
                    throw ex;
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }).ContinueWith(t => DoWorkAsync());

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("后台服务 【 OdinBackgroundService 】 【 running 】");
            worker = DoWorkAsync();
            return ExecuteAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await worker;
            // return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information($"Service:【 Stop 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
            // _timer1?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            worker?.Dispose();
            // _timer1?.Dispose();
        }
    }
}
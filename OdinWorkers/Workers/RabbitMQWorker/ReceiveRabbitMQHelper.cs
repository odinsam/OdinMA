using System;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinInject.InjectPlugs.OdinCacheManagerInject;
using OdinPlugs.OdinInject.InjectPlugs.OdinCanalInject;
using OdinPlugs.OdinNoSql.OdinRabbitMQ.Models.RabbitMQModel;
using OdinPlugs.OdinNoSql.OdinRabbitMQ.RabbitMQReceive;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinAdapterMapper;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinTime;
using OdinPlugs.OdinUtils.Utils.OdinTime;
using OdinWorkers.ErrorCodeWork;
using OdinWorkers.Models;
using RabbitMQ.Client;
using Serilog;

namespace OdinWorkers.Workers.RabbitMQWorker
{
    public class ReceiveRabbitMQHelper
    {
        private readonly IRabbitMQReceiveServer rabbitMQReceiveServer;
        private readonly IOdinCanal canalHelper;
        private readonly IOdinCacheManager cacheManager;

        public ReceiveRabbitMQHelper()
        {
            this.rabbitMQReceiveServer = OdinInjectCore.GetService<IRabbitMQReceiveServer>();
            this.canalHelper = OdinInjectCore.GetService<IOdinCanal>();
            this.cacheManager = OdinInjectCore.GetService<IOdinCacheManager>();
        }

        public void ReceiveMQ(ProjectExtendsOptions apiOptions)
        {
            Log.Information($"ReceiveRabbitMQ:【 Run 】\tTime:【{DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
            rabbitMQReceiveServer.ReceiveJsonMessage(
                apiOptions.RabbitMQ.OdinAdapter<OdinPlugs.OdinWebApi.OdinCore.ConfigModel.RabbitMQConfigModel.RabbitMQOptions, OdinPlugs.OdinNoSql.OdinRabbitMQ.Models.RabbitMQConfigModel.RabbitMQOptions>(),
                new RabbitMQReceivedModel[]
                {
                    new RabbitMQReceivedModel{
                        ExchangeName = "canal-exchange",
                        ReceiveQueues = new ReceiveQueueInfo[]{
                            new ReceiveQueueInfo{
                                QueueName = "canal-queues",
                                RoutingKey = "canal.#",
                                AutoAck = true,
                                ReceiveAction = (BasicGetResult result, IModel channel) =>
                                {
                                    var msg = RabbitMQReceiveHandler.ReceiveJsonMessageHandler(result, channel);
                                    if (!string.IsNullOrEmpty(msg))
                                    {
                                        //channel.BasicAck(1,true);
                                        var model = canalHelper.GetCanalInfo(msg);
                                        System.Console.WriteLine($"Canal-WorkService:【 Run 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】\t消息时间:【 {model.ts.ToTimer(EnumSsOrMs.ms).ToString("yyyy-MM-dd hh:mm:ss")} 】\tRouteingKey:【 {result.RoutingKey} 】");
                                        System.Console.WriteLine(msg);
                                        if (model.table == "tb_errorcode")
                                        {
                                            ErrorCodeWorker.ErrorCodeCanalHandler(canalHelper, cacheManager, model);
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new RabbitMQReceivedModel{
                        ExchangeName = "aop_ApiInvoker_Exchange",
                        ReceiveQueues = new ReceiveQueueInfo[]{
                            new ReceiveQueueInfo{
                                QueueName = "aop_ApiInvoker_Queues",
                                RoutingKey = "aop.ApiInvoker.#",
                                AutoAck = true,
                                ReceiveAction = (BasicGetResult result, IModel channel) =>
                                {
                                    var msg = RabbitMQReceiveHandler.ReceiveJsonMessageHandler(result, channel);
                                    if (!string.IsNullOrEmpty(msg))
                                    {
                                        //channel.BasicAck(1,true);
                                        var model = canalHelper.GetCanalInfo(msg);
                                        System.Console.WriteLine($"Aop_ApiInvoker-WorkService:【 Run 】\tTime:【 {DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】\t消息时间:【 {model.ts.ToTimer(EnumSsOrMs.ms).ToString("yyyy-MM-dd hh:mm:ss")} 】\tRouteingKey:【 {result.RoutingKey} 】");
                                        System.Console.WriteLine(msg);
                                    }
                                }
                            }
                        }
                    }
                }
            );
        }
    }
}
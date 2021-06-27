using System;
using Odin.Plugs.OdinBasicDataType.OdinEnum;
using Odin.Plugs.OdinCore.Models.RabbitMQModel;
using Odin.Plugs.OdinExtensions.BasicExtensions.OdinTime;
using Odin.Plugs.OdinMAF.OdinCacheManager;
using Odin.Plugs.OdinMAF.OdinCanalService;
using Odin.Plugs.OdinMAF.OdinRabbitMQ.RabbitMQReceive;
using Odin.Plugs.OdinMvcCore.OdinInject;
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
            this.rabbitMQReceiveServer = OdinInjectHelper.GetService<IRabbitMQReceiveServer>();
            this.canalHelper = OdinInjectHelper.GetService<IOdinCanal>();
            this.cacheManager = OdinInjectHelper.GetService<IOdinCacheManager>();
        }

        public void ReceiveMQ(ProjectExtendsOptions apiOptions)
        {
            Log.Information($"Service:【 Run 】\tTime:【{DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss")} 】");
            rabbitMQReceiveServer.ReceiveJsonMessage(
                apiOptions.RabbitMQ,
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
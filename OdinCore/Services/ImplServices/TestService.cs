using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OdinCore.Models;
using OdinCore.Models.DbModels;
using OdinCore.Services.InterfaceServices;
using OdinPlugs.OdinInject.InjectPlugs.OdinMongoDbInject;
using OdinPlugs.OdinInject.InjectPlugs.OdinCacheManagerInject;
using OdinPlugs.OdinInject.InjectPlugs.OdinRedisInject;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinEFCore.EntityFrameworkExtensions.EFExtensions;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinExtensions;
using OdinPlugs.OdinWebApi.OdinCore.Models;
using OdinPlugs.OdinEFCore.EntityFrameworkExtensions.EFInterface;
using OdinPlugs.OdinWebApi.OdinMvcCore.ServicesCore.ServicesExtensions;
using OdinPlugs.OdinMQ.OdinRabbitMQ.RabbitMQReceive;
using OdinPlugs.OdinModels.ErrorCode;

namespace OdinCore.Services.ImplServices
{
    public class TestService : SqlSugarBaseRepository<ErrorCode_DbModel>, ITestService
    {
        private readonly IInerService innerService;

        #region 内部私有变量
        private readonly IOptionsSnapshot<ProjectExtendsOptions> iApiOptions;
        private readonly ProjectExtendsOptions apiOptions;
        private readonly IOdinMongo mongoHelper;
        private readonly IOdinRedis redisCacheHelper;
        private readonly IOdinCacheManager odinCacheManager;
        private readonly IRabbitMQReceiveServer rabbitMQReceiveServer;
        #endregion

        public TestService()
        {
            this.innerService = MvcContext.GetRequiredServices<IInerService>();
            this.iApiOptions = OdinInjectCore.GetService<IOptionsSnapshot<ProjectExtendsOptions>>();
            this.apiOptions = iApiOptions.Value;
            this.mongoHelper = OdinInjectCore.GetService<IOdinMongo>();
            this.redisCacheHelper = OdinInjectCore.GetService<IOdinRedis>();
            this.odinCacheManager = OdinInjectCore.GetService<IOdinCacheManager>();
            this.rabbitMQReceiveServer = OdinInjectCore.GetService<IRabbitMQReceiveServer>();
        }

        public OdinActionResult SelectErrorCode(long id)
        {
            return null;
            //return this.ServiceResult(base.QueryByIdAsync(id, true).Result);
        }


        public OdinActionResult DeleteErrorCode(long id)
        {
            return this.ServiceOk();
        }

        public IBaseRepository<T> GetRepository<T>(DbContext _objectContext) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public OdinActionResult InsertErrorCode(ErrorCode_Model errorCode)
        {
            throw new NotImplementedException();
        }

        public OdinActionResult SelectErrorCodeByCache(string key)
        {
            throw new NotImplementedException();
        }
        public OdinActionResult show(long id)
        {

            System.Console.WriteLine("this is TestService show method");

            return this.innerService.show(id);
        }

        public OdinActionResult showPost()
        {
            throw new NotImplementedException();
        }

        public OdinActionResult UpdateErrorCode(ErrorCode_Model errorCode, long id)
        {
            throw new NotImplementedException();
        }

        // public OdinActionResult show()
        // {

        //     // get
        // var str = OdinHttpClientFactory<string>("OdinClient", "http://apis.juhe.cn/mobile/get?phone=17377770729&dtype=json&key=f0780781a15f693eadfc2165d3f22f78");
        //     // this.redisCacheHelper.Get<ErrorCode_Model>("sys-error")

        //     // rabbitmq
        //     // rabbitMQReceiveServer.ReceiveJsonMessage(apiOptions.RabbitMQ,
        //     //                                             new RabbitMQReceivedModel
        //     //                                             {
        //     //                                                 ExchangeName = "canal-exchange",
        //     //                                                 QueueName = "canal-queues",
        //     //                                                 AutoAck = false
        //     //                                             },
        //     //                                             (object obj, BasicDeliverEventArgs e) =>
        //     //                                             {
        //     //                                                 var sender = obj as IModel;
        //     //                                                 System.Console.WriteLine(sender.GetType().FullName);
        //     //                                                 var message = e.Body;//接收到的消息
        //     //                                                 Console.WriteLine("接收到信息为:" + Encoding.UTF8.GetString(message.ToArray()));
        //     //                                                 sender.BasicAck(e.DeliveryTag, false);
        //     //                                             });
        //     // var str = apiHelper.GetSSLWebApi<string>("http://apis.juhe.cn/", "/mobile/get?phone=17377770729&dtype=json&key=f0780781a15f693eadfc2165d3f22f78");

        //     // post
        //     // var obj = OdinHttpClientFactory.PostRequestAsync<OdinActionResult>("OdinClient",
        //     //                 "http://127.0.0.1:20303/api/v1/LinkTrack/pfda?id=4&name=admin",
        //     //                 new { User = "odinsam" });
        //     // return this.ServiceResult(obj.Result.Data);
        // }

        // public OdinActionResult showPost()
        // {
        //     this.errorCodes.Add(new ErrorCode_DbModel { ErrorCode = "testCode", CodeShowMessage = "test code", CodeErrorMessage = "test code" });
        //     return this.ServiceDbSave(this.entity.SaveChanges(),
        //                     this.ServiceOk(),
        //                     this.ServiceResult("db save error"));
        // }

        // public OdinActionResult InsertErrorCode(ErrorCode_Model errorCode)
        // {

        //     int i = this.errorCodes.Add(new ErrorCode_DbModel { ErrorCode = errorCode.ErrorCode, CodeShowMessage = errorCode.ShowMessage, CodeErrorMessage = errorCode.ErrorMessage });
        //     return this.ServiceDbSave(i,
        //                     this.ServiceOk(),
        //                     this.ServiceResult("db save error"));
        // }

        // public OdinActionResult UpdateErrorCode(ErrorCode_Model errorCode, int id)
        // {
        //     var dbModel = this.errorCodes.Get(e => e.Id == id);
        //     dbModel.ErrorCode = errorCode.ErrorCode;
        //     dbModel.CodeShowMessage = errorCode.ShowMessage;
        //     dbModel.CodeErrorMessage = errorCode.ErrorMessage;
        //     int i = this.errorCodes.Edit(dbModel);
        //     return this.ServiceDbSave(i,
        //                     this.ServiceOk(),
        //                     this.ServiceResult("db save error"));
        // }

        // public OdinActionResult DeleteErrorCode(int id)
        // {
        //     var dbModel = this.errorCodes.Get(e => e.Id == id);
        //     int i = this.errorCodes.Delete(dbModel);
        //     return this.ServiceDbSave(i,
        //                     this.ServiceOk(),
        //                     this.ServiceResult("db save error"));
        // }

        // public OdinActionResult SelectErrorCode(int id)
        // {
        //     return this.ServiceResult(this.errorCodes.Get(e => e.Id == id));
        // }

        // public OdinActionResult SelectErrorCodeByCache(string key)
        // {
        //     var model = this.odinCacheManager.Get<ErrorCode_Model>(key);
        //     return this.ServiceResult(model);
        // }
    }
}
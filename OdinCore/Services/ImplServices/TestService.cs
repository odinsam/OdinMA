using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Odin.Plugs.OdinCore.Models;
using Odin.Plugs.OdinCore.Models.ErrorCode;
using Odin.Plugs.OdinMAF.OdinCacheManager;
using Odin.Plugs.OdinMAF.OdinEF.EFCore.EFExtensions;
using Odin.Plugs.OdinMAF.OdinEF.EFCore.EFExtensions.EFInterface;
using Odin.Plugs.OdinMAF.OdinMongoDb;
using Odin.Plugs.OdinMAF.OdinRabbitMQ.RabbitMQReceive;
using Odin.Plugs.OdinMAF.OdinRedis;
using Odin.Plugs.OdinMvcCore.MvcCore;
using Odin.Plugs.OdinMvcCore.OdinInject;
using Odin.Plugs.OdinMvcCore.ServicesCore.ServicesExtensions;
using OdinCore.Models;
using OdinCore.Models.DbModels;
using OdinCore.Services.InterfaceServices;

namespace OdinCore.Services.ImplServices
{
    public class TestService : SqlSugarBaseRepository<ErrorCode_DbModel>, ITestService
    {

        #region 内部私有变量
        private readonly IOptionsSnapshot<ProjectExtendsOptions> iApiOptions;
        private readonly ProjectExtendsOptions apiOptions;
        private readonly IMapper mapper;
        private readonly IOdinMongo mongoHelper;
        private readonly IOdinRedisCache redisCacheHelper;
        private readonly IOdinCacheManager odinCacheManager;
        private readonly IMvcApiCore mvcApiCore;
        private readonly IRabbitMQReceiveServer rabbitMQReceiveServer;
        #endregion

        public TestService()
        {
            this.iApiOptions = OdinInjectHelper.GetService<IOptionsSnapshot<ProjectExtendsOptions>>();
            this.apiOptions = iApiOptions.Value;
            this.mapper = OdinInjectHelper.GetService<IMapper>();
            this.mongoHelper = OdinInjectHelper.GetService<IOdinMongo>();
            this.redisCacheHelper = OdinInjectHelper.GetService<IOdinRedisCache>();
            this.odinCacheManager = OdinInjectHelper.GetService<IOdinCacheManager>();
            this.mvcApiCore = OdinInjectHelper.GetService<IMvcApiCore>();
            this.rabbitMQReceiveServer = OdinInjectHelper.GetService<IRabbitMQReceiveServer>();
        }

        public OdinActionResult SelectErrorCode(long id)
        {
            return this.ServiceResult(base.QueryByIdAsync(id, true).Result);
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

        public OdinActionResult show()
        {
            throw new NotImplementedException();
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
        //     // var str = OdinHttpClientFactory.GetRequestAsync<string>("OdinClient", "http://apis.juhe.cn/mobile/get?phone=17377770729&dtype=json&key=f0780781a15f693eadfc2165d3f22f78");
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
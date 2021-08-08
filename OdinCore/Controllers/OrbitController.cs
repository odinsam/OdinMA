using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OdinCore.Models;
using OdinCore.Models.DbModels;
using SqlSugar;
using AutoMapper;
using SqlSugar.IOC;
using Newtonsoft.Json;
using Serilog;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinString;
using OdinPlugs.OdinInject;
using OdinPlugs.OdinInject.InjectPlugs.OdinMongoDbInject;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinAttr;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinRoute;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinFilter;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinExtensions;
using OdiOdinPlugs.OdinWebApinPlugs.OdinMvcCore.MvcCore;
using OdinPlugs.OdinWebApi.OdinCore.Models.Aop;
using OdinPlugs.OdinWebApi.OdinCore.Models;
using OdinPlugs.OdinNoSql.OdinRabbitMQ.RabbitMQSend;

namespace OdinCore.Controllers
{
    [Author("dingjj")]
    [CreateTime("2021-06-15 00:02:02", "yyyy-MM-dd HH:mm:ss")]
    [OdinControllerRoute("Orbit", "1.0")]
    [EnableCors("AllowSpecificOrigin")]
    [NoGuid]           // 请求链路检测
    [NoToken]          // 无需token检测
    [NoCheckIp]        // 访问ip检测
    [NoApiSecurity]    // 返回内容不加密
    [NoApi]            // 无需token检测
    [NoParamSignCheck] // api url 参数不验签
    [ApiController]
    // [ApiExplorerSettings(GroupName = "Orbit")]
    public class OrbitController : Controller
    {
        private readonly ProjectExtendsOptions options;
        private readonly IMapper mapper;
        private readonly SqlSugarClient entity;
        private readonly IOdinMongo mongoHelper;
        private readonly IRabbitMQSendServer rabbitMQSendHelper;
        private string guid = string.Empty;



        public OrbitController()
        {
            this.options = OdinInjectCore.GetService<IOptionsSnapshot<ProjectExtendsOptions>>().Value;
            this.entity = DbScoped.Sugar;
            this.mongoHelper = OdinInjectCore.GetService<IOdinMongo>();
            this.rabbitMQSendHelper = OdinInjectCore.GetService<IRabbitMQSendServer>();
        }

        [NoApiFilter]  // 跳过全局拦截
        [NoGuidFilter]  // 跳过访问链路检测
        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [NoParamSignCheckFilter] // 跳过api url 参数验签
        [OdinActionRoute("ApiInvokerRecord", "1.0")]
        [HttpPost]
        public IActionResult ApiInvokerRecord()
        {
            // todo ApiInvokerRecord 方法开始 
            // ^    获取action所需参数 
            var requestParams = this.GetRequestParams();
            var api = OdinApiCommentCore.GetApiComment(Program.ApiComments, this.GetType().Name, this.RouteData.Values["action"].ToString());
            var model = JsonConvert.DeserializeObject<Aop_ApiInvokerRecord_Model>(JsonConvert.SerializeObject(requestParams.RequestJson));
            var dbModel = mapper.Map<Aop_ApiInvokerRecord_DbModel>(model);
            try
            {
                if (entity.Insertable<Aop_ApiInvokerRecord_DbModel>(dbModel).ExecuteCommand() > 0)
                    System.Console.WriteLine(" ApiInvokerRecord 保存『 成功 』");
                return this.Ok();
            }
            catch (Exception ex)
            {
                Log.Error(JsonConvert.SerializeObject(ex).ToJsonFormatString());
                // ! 方法失败,返回错误码以及错误信息 
                this.mongoHelper.AddModel("ApiInvokerRecord", dbModel);
                return this.OdinResult(null, "操作记录保存 『 报错 』");
            }
        }



        [NoApiFilter]  // 跳过全局拦截
        [NoGuidFilter]  // 跳过访问链路检测
        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [NoParamSignCheckFilter] // 跳过api url 参数验签
        [OdinActionRoute("ApiInvokerCatch", "1.0")]
        [HttpPost]
        public IActionResult ApiInvokerCatch()
        {
            // todo ApiInvokerRecord 方法开始 
            // ^    获取action所需参数 
            var requestParams = this.GetRequestParams();
            var api = OdinApiCommentCore.GetApiComment(Program.ApiComments, this.GetType().Name, this.RouteData.Values["action"].ToString());
            var model = JsonConvert.DeserializeObject<Aop_ApiInvokerCatch_Model>(JsonConvert.SerializeObject(requestParams.RequestJson));
            var dbModel = mapper.Map<Aop_ApiInvokerCatch_DbModel>(model);
            try
            {
                if (entity.Insertable<Aop_ApiInvokerCatch_DbModel>(dbModel).ExecuteCommand() > 0)
                    System.Console.WriteLine(" ApiInvokerCatch 保存『 成功 』");
                return this.Ok();
            }
            catch (Exception ex)
            {
                Log.Error(JsonConvert.SerializeObject(ex).ToJsonFormatString());
                // ! 方法失败,返回错误码以及错误信息 
                this.mongoHelper.AddModel("ApiInvokerRecord", dbModel);
                return this.OdinResult(null, "操作记录保存 『 报错 』");
            }
        }


        [NoApiFilter]  // 跳过全局拦截
        [NoGuidFilter]  // 跳过访问链路检测
        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [NoParamSignCheckFilter] // 跳过api url 参数验签
        [OdinActionRoute("ApiInvokerException", "1.0")]
        [HttpPost]
        public IActionResult ApiInvokerException()
        {

            // todo ApiInvokerRecord 方法开始 
            // ^    获取action所需参数 
            var requestParams = this.GetRequestParams();
            var api = OdinApiCommentCore.GetApiComment(Program.ApiComments, this.GetType().Name, this.RouteData.Values["action"].ToString());
            var model = JsonConvert.DeserializeObject<Aop_ApiInvokerCatch_Model>(JsonConvert.SerializeObject(requestParams.RequestJson));
            var dbModel = mapper.Map<Aop_ApiInvokerCatch_DbModel>(model);
            try
            {
                if (entity.Insertable<Aop_ApiInvokerCatch_DbModel>(dbModel).ExecuteCommand() > 0)
                    System.Console.WriteLine(" ApiInvokerCatch 保存『 成功 』");
                return this.Ok();
            }
            catch (Exception ex)
            {
                Log.Error(JsonConvert.SerializeObject(ex).ToJsonFormatString());
                // ! 方法失败,返回错误码以及错误信息 
                this.mongoHelper.AddModel("ApiInvokerRecord", dbModel);
                return this.OdinResult(null, "操作记录保存 『 报错 』");
            }
        }
    }
}
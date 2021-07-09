using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OdinCore.Models;
using OdinCore.Models.DbModels;
using OdinCore.Services.InterfaceServices;
using SqlSugar.IOC;
using OdinPlugs.OdinMvcCore.OdinRoute;
using OdinPlugs.OdinMvcCore.OdinFilter;
using OdinPlugs.OdinMvcCore.MvcCore;
using OdinPlugs.OdinMAF.OdinCacheManager;
using OdinPlugs.OdinMAF.OdinMongoDb;
using OdinPlugs.OdinMvcCore.OdinInject;
using OdinPlugs.OdinCore.Models.ErrorCode;
using OdinPlugs.OdinMvcCore.OdinValidate.ApiParamsValidate;
using OdinPlugs.OdinCore.Models;
using OdinPlugs.OdinMvcCore.OdinExtensions;
using OdinPlugs.OdinBasicDataType.OdinEnum;
using OdinPlugs.OdinNetCore.WebApi.HttpClientHelper;
using OdinPlugs.OdinMAF.OdinCapService;
using OdinPlugs.OdinMvcCore.OdinAttr;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinString;

namespace OdinCore.Controllers
{
    [ApiController]
    [Author("odinsam11")]
    [CreateTime("2021-06-07 14:09:07", "yyyy-MM-dd HH:mm:ss")]
    [OdinControllerRoute("LinkTrack", "1.0")]
    [EnableCors("AllowSpecificOrigin")]
    [NoToken] // 无需token检测
    [NoCheckIp] // 访问ip检测
    [NoApiSecurity] // 返回内容不加密
    [Api] // api拦截
    [NoParamSignCheck] // api url 参数不验签
    // [ApiExplorerSettings(GroupName = "LinkTrack")]
    public partial class LinkTrackController : Controller
    {
        private readonly IOptionsSnapshot<ProjectExtendsOptions> iApiOptions;
        public ProjectExtendsOptions apiOptions { get; set; }
        private readonly IMapper mapper;
        private readonly ITestService testService;
        private readonly IOdinCacheManager cacheManager;
        private readonly ICapPublisher capBus;
        private readonly IOdinCapEventBus odinCapEventBus;
        private readonly IOdinMongo mongoHelper;
        private string guid = string.Empty;
        #region 构造函数
        public LinkTrackController()
        {
            this.iApiOptions = OdinInjectHelper.GetService<IOptionsSnapshot<ProjectExtendsOptions>>();
            this.apiOptions = iApiOptions.Value;
            this.mapper = OdinInjectHelper.GetService<IMapper>();
            this.testService = OdinInjectHelper.GetService<ITestService>();
            this.cacheManager = OdinInjectHelper.GetService<IOdinCacheManager>();
            this.capBus = OdinInjectHelper.GetService<ICapPublisher>();
            this.odinCapEventBus = OdinInjectHelper.GetService<IOdinCapEventBus>();
            this.mongoHelper = OdinInjectHelper.GetService<IOdinMongo>();
        }

        public class SourceC
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [ApiFilter]
        [OdinActionRoute("TestAction_v2_in_v1/{errorCode}/{id}", "2.0")]
        [HttpPost]
        [Consumes("application/json")]
        public IActionResult PostTestAction(
                    [FromBody][Required] ErrorCode_Model error, [FromRoute][Required][OdinSnowFlakeValidation] long id)
        {
            return this.OdinResult("1.0");
        }

        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [ApiFilter]
        [OdinActionRoute("TestActionFromData/{id}", "1.0")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult PostTestActionFromData([FromForm] ErrorCode_Model error)
        {
            System.Console.WriteLine(JsonConvert.SerializeObject(error).ToJsonFormatString());
            return this.OdinResult("multipart/form-data method");
        }

        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [ApiFilter]
        [OdinActionRoute("TestAction/{id}", "1.0")]
        [HttpPost]
        public IActionResult PostTestAction(
            [FromBody][Required] ErrorCode_Model error, string errorCode, [FromRoute][Required][OdinSnowFlakeValidation] long id)
        {
            return this.OdinResult("this is version 2.0 method");
        }

        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [OdinActionRoute("TestAction/{errorCode}", "1.0")]
        [HttpGet]
        public IActionResult TestAction(string errorCode, int id)
        {
            System.Console.WriteLine(id);
            var db = DbScoped.Sugar;
            foreach (var item in this.mongoHelper.GetAllCollectionNames())
            {
                System.Console.WriteLine(item);
            }

            return this.OdinResult(db.Queryable<ErrorCode_DbModel>().ToList());

            // ^ cap 
            // System.Console.WriteLine(apiOptions.Cap.GetAopRouteingKey("testKey"));
            // var header = new Dictionary<string, string>()
            // {
            //     ["RouteingKey"] = "cap.odinCore.Aop.RabbitMQ.TestAction",
            // };
            // OdinCapHelper.CapPublish("cap.odinCore.Aop.RabbitMQ.TestAction", DateTime.Now, () =>
            // {
            //     System.Console.WriteLine("to do something");
            // }, header);
            // return this.OdinResultOk();

            // ^    异常
            var requestParams = this.GetRequestParams();
            JObject obj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(OdinHttpClientFactory.ConvertPostDataToDictionary<string>(requestParams.RequestQueryString)));
            // guid = HttpContext.Request.Headers["Guid"].ToString();
            var api = OdinApiCommentCore.GetApiComment(Program.ApiComments, this.GetType().Name, this.RouteData.Values["action"].ToString());
            // var validate = mvcCore.ValidateParams(guid, api, obj, enumMethod.Post, "errorCode");
            // if (validate != null)
            //     // ! 验证失败,返回失败错误码 
            //     return validate;
            // if (!cacheManager.Exists(errorCode))
            //     throw new OdinExtendsException("错误码不存在");
            // return this.OdinResult(cacheManager.Get<ErrorCode_Model>(errorCode));

            // ^    catch
            try
            {
                if (!cacheManager.Exists(errorCode))
                    throw new Exception("错误码不存在");
                return this.OdinResult(cacheManager.Get<ErrorCode_Model>(errorCode));
            }
            catch (Exception ex)
            {
                return this.OdinCatchResult(ex, "sys-error");
            }
        }
        #endregion

        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [OdinActionRoute("SubscribeAction", "1.0")]
        [CapSubscribe("cap.odinCore.Aop.RabbitMQ.#")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Task> CheckReceivedMessage(DateTime time, [FromCap] CapHeader header)
        {
            Console.WriteLine($"============{header["RouteingKey"]}==========={time.ToString("yyyy-MM-dd hh:mm:ss")}================");
            return Task.CompletedTask;
        }
    }
}
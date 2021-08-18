using System.Security;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OdinCore.Services.InterfaceServices;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinInject.InjectPlugs;
using OdinPlugs.OdinModels.ErrorCode;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinObject;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinString;
using OdinPlugs.OdinUtils.OdinJson.ContractResolver;
using OdinPlugs.OdinWebApi.OdinCore.Models;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinFilter;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinRoute;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinValidate.ApiParamsValidate;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinWebHost;
using OdinPlugs.SnowFlake.SnowFlakePlugs.ISnowFlake;
using Microsoft.AspNetCore.Authorization;
using OdinCore.Models;
using Microsoft.Extensions.Options;
using OdinPlugs.OdinModels.ConfigModel;

namespace OdinCore.Controllers
{

    public enum EnumTest
    {
        one,
        two
    }

    [OdinControllerRoute("LinkTrack", "2.0")]
    public partial class LinkTrackController : Controller
    {
        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [ApiFilter]
        [OdinActionRoute("TestAction_v1_in_v2", "1.0")]
        [HttpPost]
        [Consumes("application/json")]
        [AllowAnonymous]
        public IActionResult PostTestActionV2([FromBody][Required] ErrorCode_Model error, [FromRoute][Required][OdinSnowFlakeValidation] long id)
        {
            return this.OdinResultOk();
        }

        /// <summary>
        /// show method
        /// </summary>
        /// <param name="error">errorCode model</param>
        /// <param name="id">雪花Id</param>
        /// <returns>method test string</returns>
        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [ApiFilter]
        [OdinActionRoute("Show", "2.0")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [OdinAuthorize]
        [ProducesResponseType(typeof(Stu), 200)]
        [AllowAnonymous]
        public IActionResult Show([FromForm][Required] EnumTest error, [FromQuery][Required] long id)
        {
            //return this.OdinResult(OdinInjectCore.GetService<IOdinCacheManager>().Get<ErrorCode_Model>("sys-allowip"));
            return this.OdinResult(OdinInjectCore.GetService<IOptionsSnapshot<ConfigOptions>>().Value);
            // var stu = new Stu
            // {
            //     id = OdinInjectCore.GetService<IOdinSnowFlake>().CreateSnowFlakeId(),
            //     name = "odinsam",
            //     age = 20
            // };
            // Console.WriteLine("student info");
            // Console.WriteLine(JsonConvert.SerializeObject(stu).ToJsonFormatString());
            // // throw new Exception("test exception");
            // return this.GetDIServices<ITestService>().show(id);
            // return this.OdinResultOk();

            // return this.OdinResult(stu);
            // this.GetDIServices<ITestService>().show(id);
            // return this.OdinResult($"{DateTime.Now.ToString()}");

        }
    }

    public class Stu
    {
        /// <summary>
        /// stu id
        /// </summary>
        /// <value></value>
        [JsonConverter(typeof(JsonConverterLong))]
        public long id { get; set; }
        /// <summary>
        /// stu name
        /// </summary>
        /// <value></value>
        public string name { get; set; }
        /// <summary>
        /// stu age
        /// </summary>
        /// <value></value>
        public int age { get; set; }
    }
}
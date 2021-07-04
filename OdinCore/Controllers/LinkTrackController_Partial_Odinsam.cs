using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OdinCore.Models.OdinInterceptor;
using OdinCore.Services.InterfaceServices;
using OdinPlugs.OdinCore.Models;
using OdinPlugs.OdinCore.Models.ErrorCode;
using OdinPlugs.OdinMAF.OdinAspectCore;
using OdinPlugs.OdinMvcCore.OdinExtensions;
using OdinPlugs.OdinMvcCore.OdinFilter;
using OdinPlugs.OdinMvcCore.OdinInject;
using OdinPlugs.OdinMvcCore.OdinRoute;
using OdinPlugs.OdinMvcCore.OdinValidate.ApiParamsValidate;

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
        public IActionResult PostTestActionV2([FromBody][Required] ErrorCode_Model error, [FromRoute][Required][OdinSnowFlakeValidation] long id)
        {
            return this.OdinResult("2.0");
        }

        /// <summary>
        /// shwo method
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

        public IActionResult Show([FromForm][Required] EnumTest error, [FromQuery][Required] long id)
        {
            // throw new Exception("test exception");
            try
            {
                this.GetDIServices<ITestService>().show(id);
                return this.OdinResult($"{DateTime.Now.ToString()}");
            }
            catch (Exception ex)
            {
                return this.OdinCatchResult(ex);
            }

        }
    }
}
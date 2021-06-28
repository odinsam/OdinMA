using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Odin.Plugs.OdinCore.Models;
using Odin.Plugs.OdinCore.Models.ErrorCode;
using Odin.Plugs.OdinMvcCore.OdinFilter;
using Odin.Plugs.OdinMvcCore.OdinRoute;
using Odin.Plugs.OdinMvcCore.OdinValidate.ApiParamsValidate;

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
        public IActionResult Show([FromForm][Required] EnumTest error, [FromQuery][Required][OdinSnowFlakeValidation] long id)
        {
            // throw new Exception("test exception");
            // 
            try
            {
                throw new Exception("test exception");
                System.Console.WriteLine(error);
                System.Console.WriteLine(id);
                return this.OdinResult("2.0");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return this.OdinCatchResult(ex);
            }

        }
    }
}
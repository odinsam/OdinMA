using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OdinPlugs.OdinInject.InjectCore;
using OdinPlugs.OdinWebApi.OdinCore.Models;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinAttr;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinFilter;
using OdinPlugs.OdinWebApi.OdinMvcCore.OdinRoute;

namespace OdinIds.Controllers
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
    public class AddIdsController : Controller
    {
        public AddIdsController()
        {
        }

        /// <summary>
        /// show method
        /// </summary>
        /// <returns>method test string</returns>
        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [ApiFilter]
        [OdinActionRoute("apiGet", "1.0")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult apiGet()
        {
            return this.OdinResultOk();
        }

        /// <summary>
        /// show method
        /// </summary>
        /// <returns>method test string</returns>
        [NoTokenFilter] // 跳过Token认证
        [NoCheckIpFilter] // 跳过访问Ip检测
        [NoApiSecurityFilter] // 跳过 api result 加密
        [ApiFilter]
        [OdinActionRoute("AddApiScopes", "1.0")]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult AddApiScopes()
        {
            var context = OdinInjectCore.GetService<ConfigurationDbContext>();
            var apiScopes = new IdentityServer4.Models.ApiScope("api2", "My API-2");
            context.ApiScopes.Add(apiScopes.ToEntity());
            context.SaveChanges();
            return this.OdinResultOk();
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Odin.Plugs.OdinMvcCore.OdinFilter;
using Odin.Plugs.OdinMvcCore.OdinInject;
using Odin.Plugs.OdinMvcCore.OdinRoute;
using OdinCore.Models;
using Odin.Plugs.OdinCore.OdinAttr;

namespace OdinCore.Controllers
{
    [Author("dingjj")]
    [CreateTime("21-06-08 23:33:13")]
    [OdinControllerRoute("Test", "1.0")]
    [EnableCors("AllowSpecificOrigin")]
    [NoGuid]           // 请求链路检测
    [NoToken]          // 无需token检测
    [NoCheckIp]        // 访问ip检测
    [NoApiSecurity]    // 返回内容不加密
    [NoApi]            // 无需token检测
    [NoParamSignCheck] // api url 参数不验签
    [ApiController]
    // [ApiExplorerSettings(GroupName = "Test")]
    public class TestController : Controller
    {
        private readonly IOptionsSnapshot<ProjectExtendsOptions> iApiOptions;
        private readonly ProjectExtendsOptions apiIOptions;
        private readonly IMapper mapper;
        #region 构造函数
        public TestController()
        {
            this.iApiOptions = OdinInjectHelper.GetService<IOptionsSnapshot<ProjectExtendsOptions>>();
            this.apiIOptions = iApiOptions.Value;
            this.mapper = OdinInjectHelper.GetService<IMapper>();
        }
        #endregion


    }
}
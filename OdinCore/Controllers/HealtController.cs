using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OdinCore.Controllers
{
    [Route("api/Health")]
    public class HealthController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult OK()
        {
            return Ok();
        }
    }
}
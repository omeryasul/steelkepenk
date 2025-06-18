using Microsoft.AspNetCore.Mvc;

namespace WEBProje.WEBApi.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Admin API çalışıyor.");
        }
    }
}

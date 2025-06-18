using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Common.Models;

namespace WEBProje.WEBApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private ISender _mediator = null!;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null)
                return NotFound();
            if (result.Succeeded && result.Data != null)
                return Ok(result.Data);
            if (result.Succeeded && result.Data == null)
                return NotFound();
            return BadRequest(result.Errors);
        }
    }
}
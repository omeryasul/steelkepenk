using Application.Common.Models;
using Application.Features.PageSettings.Commands.Create;
using Application.Features.PageSettings.Commands.Update;
using Application.Features.PageSettings.Dto;
using Application.Features.PageSettings.Queries.GetByKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEBProje.WEBApi.Controllers
{
    public class PageSettingsController : BaseApiController
    {
        [HttpGet("{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var result = await Mediator.Send(new GetPageSettingByKeyQuery(key));
            return HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePageSettingCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{key}")]
        public async Task<IActionResult> Update(string key, UpdatePageSettingCommand command)
        {
            if (key != command.Key)
                return BadRequest("Key mismatch");

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}
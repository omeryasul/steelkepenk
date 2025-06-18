using Application.Common.Models;
using Application.Features.ContactMessages.Commands.Create;
using Application.Features.ContactMessages.Commands.Delete;
using Application.Features.ContactMessages.Commands.Update;
using Application.Features.ContactMessages.DTOs;
using Application.Features.ContactMessages.Queries.GetAll;
using Application.Features.ContactMessages.Queries.GetById;
using Application.Features.ContactMessages.Queries.GetStats;
using Application.Features.ContactMessages.Queries.GetUnread;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Api.Controllers
{
    /// <summary>
    /// İletişim mesajları yönetimi için API endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContactMessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContactMessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tüm iletişim mesajlarını sayfalı olarak getirir (Admin)
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<ContactMessageListDto>>>> GetContactMessages(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? sortBy = "CreatedDate",
            [FromQuery] bool sortDescending = true)
        {
            var query = new GetContactMessagesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Status = !string.IsNullOrEmpty(status) ? Enum.Parse<Domain.Enums.ContactMessageStatus>(status, true) : null,
                StartDate = startDate,
                EndDate = endDate,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<ContactMessageListDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Belirli bir iletişim mesajını ID ile getirir (Admin)
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ContactMessageDetailDto>>> GetContactMessage(int id)
        {
            var query = new GetContactMessageByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(ApiResponse<ContactMessageDetailDto>.ErrorResult("Mesaj bulunamadı."));

            return Ok(ApiResponse<ContactMessageDetailDto>.SuccessResult(result));
        }

        /// <summary>
        /// Okunmamış mesajları getirir (Admin)
        /// </summary>
        [HttpGet("unread")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<ContactMessageListDto>>>> GetUnreadMessages(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetUnreadContactMessagesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<ContactMessageListDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Mesaj istatistiklerini getirir (Admin)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ContactMessageStatsDto>>> GetMessageStatistics()
        {
            var query = new GetContactMessageStatsQuery();
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<ContactMessageStatsDto>.SuccessResult(result));
        }

        /// <summary>
        /// Son mesajları getirir (Admin dashboard için)
        /// </summary>
        [HttpGet("recent")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<ContactMessageListDto>>>> GetRecentMessages([FromQuery] int count = 5)
        {
            var query = new GetContactMessagesQuery
            {
                PageSize = count,
                SortBy = "CreatedDate",
                SortDescending = true
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<ContactMessageListDto>>.SuccessResult(result.Items.ToList()));
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateContactMessage([FromBody] CreateContactMessageCommand command)
        {
            // IP adresi ve User Agent bilgilerini ekle
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var commandWithClientInfo = command with
            {
                IpAddress = clientIp,
                UserAgent = userAgent
            };

            var result = await _mediator.Send(commandWithClientInfo);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<int>.ErrorResult(result.Errors.First()));

            return CreatedAtAction(
                nameof(GetContactMessage),
                new { id = result.Data },
                ApiResponse<int>.SuccessResult(result.Data, "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız."));
        }

        /// <summary>
        /// Mesajı günceller (Admin)
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateContactMessage(int id, [FromBody] UpdateContactMessageCommand command)
        {
            if (id != command.Id)
                return BadRequest(ApiResponse<bool>.ErrorResult("ID uyuşmazlığı."));

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Mesaj başarıyla güncellendi."));
        }

        /// <summary>
        /// Mesaja yanıt verir (Admin)
        /// </summary>
        [HttpPost("{id:int}/reply")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ReplyToMessage(int id, [FromBody] ReplyToContactMessageCommand command)
        {
            if (id != command.Id)
                return BadRequest(ApiResponse<bool>.ErrorResult("ID uyuşmazlığı."));

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Yanıt başarıyla gönderildi."));
        }

        /// <summary>
        /// Mesajı okundu olarak işaretler (Admin)
        /// </summary>
        [HttpPost("{id:int}/mark-read")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(int id)
        {
            var command = new MarkAsReadCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Mesaj okundu olarak işaretlendi."));
        }

        /// <summary>
        /// Mesajı siler (Admin)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteContactMessage(int id)
        {
            var command = new DeleteContactMessageCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Mesaj başarıyla silindi."));
        }

        /// <summary>
        /// Birden fazla mesajı toplu olarak siler (Admin)
        /// </summary>
        [HttpDelete("bulk")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> BulkDeleteContactMessages([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return BadRequest(ApiResponse<bool>.ErrorResult("Silinecek mesaj seçilmedi."));

            var successCount = 0;
            var errors = new List<string>();

            foreach (var id in ids)
            {
                try
                {
                    var command = new DeleteContactMessageCommand(id);
                    var result = await _mediator.Send(command);

                    if (result.Succeeded)
                        successCount++;
                    else
                        errors.AddRange(result.Errors);
                }
                catch (Exception ex)
                {
                    errors.Add($"ID {id}: {ex.Message}");
                }
            }

            if (errors.Any())
            {
                return Ok(ApiResponse<bool>.ErrorResult(
                    $"{successCount}/{ids.Length} mesaj silindi. Hatalar: {string.Join(", ", errors)}"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true, $"{successCount} mesaj başarıyla silindi."));
        }
    }
}
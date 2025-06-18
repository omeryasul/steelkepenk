using Application.Common.Models;
using Application.Features.Tags.Commands.Create;
using Application.Features.Tags.Commands.Delete;
using Application.Features.Tags.Commands.Update;
using Application.Features.Tags.DTOs;
using Application.Features.Tags.Queries.GetAll;
using Application.Features.Tags.Queries.GetById;
using Application.Features.Tags.Queries.GetBySlug;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WEB.Api.Controllers
{
    /// <summary>
    /// Etiket yönetimi için API endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tüm etiketleri sayfalı olarak getirir
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TagDto>>>> GetTags(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = "Name",
            [FromQuery] bool sortDescending = false)
        {
            var query = new GetTagsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                IsActive = isActive,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<TagDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Aktif tüm etiketleri getirir (Select box için)
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<List<TagDto>>>> GetActiveTags()
        {
            var query = new GetTagsQuery
            {
                IsActive = true,
                PageSize = 1000,
                SortBy = "Name"
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<TagDto>>.SuccessResult(result.Items.ToList()));
        }

        /// <summary>
        /// Belirli bir etiketi ID ile getirir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<TagDto>>> GetTag(int id)
        {
            var query = new GetTagByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(ApiResponse<TagDto>.ErrorResult("Etiket bulunamadı."));

            return Ok(ApiResponse<TagDto>.SuccessResult(result));
        }

        /// <summary>
        /// Belirli bir etiketi slug ile getirir
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        public async Task<ActionResult<ApiResponse<TagDto>>> GetTagBySlug(string slug)
        {
            var query = new GetTagBySlugQuery(slug);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(ApiResponse<TagDto>.ErrorResult("Etiket bulunamadı."));

            return Ok(ApiResponse<TagDto>.SuccessResult(result));
        }

        /// <summary>
        /// Popüler etiketleri getirir (en çok kullanılan)
        /// </summary>
        [HttpGet("popular")]
        public async Task<ActionResult<ApiResponse<List<TagDto>>>> GetPopularTags([FromQuery] int count = 20)
        {
            var query = new GetTagsQuery
            {
                IsActive = true,
                PageSize = count,
                SortBy = "Name", // UsageCount olmadığı için Name ile sıralıyoruz
                SortDescending = false
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<TagDto>>.SuccessResult(result.Items.ToList()));
        }

        /// <summary>
        /// Etiket arama önerileri getirir
        /// </summary>
        [HttpGet("suggestions")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetTagSuggestions([FromQuery] string? term = null)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
                return Ok(ApiResponse<List<string>>.SuccessResult(new List<string>()));

            var query = new GetTagsQuery
            {
                SearchTerm = term,
                IsActive = true,
                PageSize = 10,
                SortBy = "Name"
            };

            var result = await _mediator.Send(query);
            var suggestions = result.Items.Select(t => t.Name).ToList();

            return Ok(ApiResponse<List<string>>.SuccessResult(suggestions));
        }

        /// <summary>
        /// Yeni etiket oluşturur (Admin)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> CreateTag([FromBody] CreateTagCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<int>.ErrorResult(result.Errors.First()));

            return CreatedAtAction(
                nameof(GetTag),
                new { id = result.Data },
                ApiResponse<int>.SuccessResult(result.Data, "Etiket başarıyla oluşturuldu."));
        }

        /// <summary>
        /// Etiketi günceller (Admin)
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateTag(int id, [FromBody] UpdateTagCommand command)
        {
            if (id != command.Id)
                return BadRequest(ApiResponse<bool>.ErrorResult("ID uyuşmazlığı."));

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Etiket başarıyla güncellendi."));
        }

        /// <summary>
        /// Etiketi siler (Admin)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTag(int id)
        {
            var command = new DeleteTagCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Etiket başarıyla silindi."));
        }

        /// <summary>
        /// Birden fazla etiketi toplu olarak siler (Admin)
        /// </summary>
        [HttpDelete("bulk")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> BulkDeleteTags([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return BadRequest(ApiResponse<bool>.ErrorResult("Silinecek etiket seçilmedi."));

            var successCount = 0;
            var errors = new List<string>();

            foreach (var id in ids)
            {
                try
                {
                    var command = new DeleteTagCommand(id);
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
                    $"{successCount}/{ids.Length} etiket silindi. Hatalar: {string.Join(", ", errors)}"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true, $"{successCount} etiket başarıyla silindi."));
        }

        /// <summary>
        /// Etiket slug'ının benzersiz olup olmadığını kontrol eder
        /// </summary>
        [HttpGet("check-slug")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckSlugAvailability(
            [FromQuery, Required] string slug,
            [FromQuery] int? excludeId = null)
        {
            var existingTag = await _mediator.Send(new GetTagBySlugQuery(slug));

            bool isAvailable = existingTag == null || (excludeId.HasValue && existingTag.Id == excludeId.Value);

            return Ok(ApiResponse<bool>.SuccessResult(isAvailable,
                isAvailable ? "Slug kullanılabilir." : "Bu slug zaten kullanımda."));
        }

        /// <summary>
        /// Etiket istatistiklerini getirir (Admin)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetTagStatistics()
        {
            var allTags = await _mediator.Send(new GetTagsQuery { PageSize = 1000 });

            var stats = new
            {
                TotalTags = allTags.TotalCount,
                ActiveTags = allTags.Items.Count(t => t.IsActive),
                InactiveTags = allTags.Items.Count(t => !t.IsActive),
                MostUsedTags = allTags.Items
                    .OrderBy(t => t.Name) // Usage count olmadığı için Name ile sıralama
                    .Take(10)
                    .Select(t => new { t.Name, t.Slug })
                    .ToList(),
                RecentTags = allTags.Items
                    .OrderByDescending(t => t.CreatedDate)
                    .Take(5)
                    .Select(t => new { t.Name, t.CreatedDate })
                    .ToList()
            };

            return Ok(ApiResponse<object>.SuccessResult(stats));
        }
    }
}
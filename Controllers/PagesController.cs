using Application.Common.Models;
using Application.Features.Contents.DTOs;
using Application.Features.Contents.Queries.GetAll;
using Application.Features.Contents.Queries.GetById;
using Application.Features.Contents.Queries.GetBySlug;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WEB.Api.Controllers
{
    /// <summary>
    /// Sayfa yönetimi için API endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Sayfa tipindeki içerikleri getirir (Page type olan Contents)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentListDto>>>> GetPages(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null)
        {
            var query = new GetContentsQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Type = Domain.Enums.ContentType.Page, // Page tipindeki içerikler
                Status = !string.IsNullOrEmpty(status) ? Enum.Parse<Domain.Enums.ContentStatus>(status, true) : null
            };

            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<PagedResult<ContentListDto>>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<PagedResult<ContentListDto>>.SuccessResult(result.Data));
        }

        /// <summary>
        /// Yayınlanan sayfaları getirir
        /// </summary>
        [HttpGet("published")]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentListDto>>>> GetPublishedPages(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetContentsQuery
            {
                Page = page,
                PageSize = pageSize,
                Type = Domain.Enums.ContentType.Page,
                Status = Domain.Enums.ContentStatus.Published
            };

            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<PagedResult<ContentListDto>>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<PagedResult<ContentListDto>>.SuccessResult(result.Data));
        }

        /// <summary>
        /// Belirli bir sayfayı ID ile getirir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ContentDetailDto>>> GetPage(int id)
        {
            var query = new GetContentByIdQuery(id);
            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return NotFound(ApiResponse<ContentDetailDto>.ErrorResult("Sayfa bulunamadı."));

            // Sadece Page tipindeki içerikleri döndür
            if (result.Data.Type != Domain.Enums.ContentType.Page)
                return NotFound(ApiResponse<ContentDetailDto>.ErrorResult("Bu bir sayfa değil."));

            return Ok(ApiResponse<ContentDetailDto>.SuccessResult(result.Data));
        }

        /// <summary>
        /// Belirli bir sayfayı slug ile getirir
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        public async Task<ActionResult<ApiResponse<ContentDetailDto>>> GetPageBySlug(string slug)
        {
            var query = new GetContentBySlugQuery(slug, onlyPublished: true);
            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return NotFound(ApiResponse<ContentDetailDto>.ErrorResult("Sayfa bulunamadı."));

            // Sadece Page tipindeki içerikleri döndür
            if (result.Data.Type != Domain.Enums.ContentType.Page)
                return NotFound(ApiResponse<ContentDetailDto>.ErrorResult("Bu bir sayfa değil."));

            return Ok(ApiResponse<ContentDetailDto>.SuccessResult(result.Data));
        }

        /// <summary>
        /// Sayfa istatistiklerini getirir (Admin)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetPageStatistics()
        {
            var allPages = await _mediator.Send(new GetContentsQuery
            {
                Type = Domain.Enums.ContentType.Page,
                PageSize = 1000
            });

            if (!allPages.Succeeded)
                return BadRequest(ApiResponse<object>.ErrorResult("İstatistikler alınamadı."));

            var stats = new
            {
                TotalPages = allPages.Data.TotalCount,
                PublishedPages = allPages.Data.Items.Count(p => p.Status == Domain.Enums.ContentStatus.Published),
                DraftPages = allPages.Data.Items.Count(p => p.Status == Domain.Enums.ContentStatus.Draft),
                RecentPages = allPages.Data.Items
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(5)
                    .Select(p => new { p.Title, p.CreatedDate, p.Status })
                    .ToList()
            };

            return Ok(ApiResponse<object>.SuccessResult(stats));
        }

        /// <summary>
        /// Sayfa slug'ının benzersiz olup olmadığını kontrol eder
        /// </summary>
        [HttpGet("check-slug")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckSlugAvailability(
            [FromQuery, Required] string slug,
            [FromQuery] int? excludeId = null)
        {
            var existingContent = await _mediator.Send(new GetContentBySlugQuery(slug, onlyPublished: false));

            bool isAvailable = !existingContent.Succeeded || (excludeId.HasValue && existingContent.Data.Id == excludeId.Value);

            return Ok(ApiResponse<bool>.SuccessResult(isAvailable,
                isAvailable ? "Slug kullanılabilir." : "Bu slug zaten kullanımda."));
        }
    }
}
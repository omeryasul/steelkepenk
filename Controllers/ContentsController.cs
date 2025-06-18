using Application.Common.Models;
using Application.Features.Contents.Commands.Create;
using Application.Features.Contents.Commands.Delete;
using Application.Features.Contents.Commands.Update;
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
    /// İçerik yönetimi için API endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tüm içerikleri sayfalı olarak getirir
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentListDto>>>> GetContents(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? type = null,
            [FromQuery] string? status = null,
            [FromQuery] bool? isFeatured = null,
            [FromQuery] string? sortBy = "CreatedDate",
            [FromQuery] string? sortDirection = "desc")
        {
            var query = new GetContentsQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                Type = !string.IsNullOrEmpty(type) ? Enum.Parse<Domain.Enums.ContentType>(type, true) : null,
                Status = !string.IsNullOrEmpty(status) ? Enum.Parse<Domain.Enums.ContentStatus>(status, true) : null,
                IsFeatured = isFeatured,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<PagedResult<ContentListDto>>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<PagedResult<ContentListDto>>.SuccessResult(result.Data));
        }

        /// <summary>
        /// Yayınlanan içerikleri getirir (public endpoint)
        /// </summary>
        [HttpGet("published")]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentListDto>>>> GetPublishedContents(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? type = null)
        {
            var query = new GetContentsQuery
            {
                Page = page,
                PageSize = pageSize,
                CategoryId = categoryId,
                Type = !string.IsNullOrEmpty(type) ? Enum.Parse<Domain.Enums.ContentType>(type, true) : null,
                Status = Domain.Enums.ContentStatus.Published,
                SortBy = "CreatedDate",
                SortDirection = "desc"
            };

            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<PagedResult<ContentListDto>>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<PagedResult<ContentListDto>>.SuccessResult(result.Data));
        }

        /// <summary>
        /// Belirli bir içeriği ID ile getirir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ContentDetailDto>>> GetContent(int id)
        {
            var query = new GetContentByIdQuery(id);
            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return NotFound(ApiResponse<ContentDetailDto>.ErrorResult("İçerik bulunamadı."));

            return Ok(ApiResponse<ContentDetailDto>.SuccessResult(result.Data));
        }

        /// <summary>
        /// Belirli bir içeriği slug ile getirir ve görüntülenme sayısını artırır
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        public async Task<ActionResult<ApiResponse<ContentDetailDto>>> GetContentBySlug(string slug)
        {
            var query = new GetContentBySlugQuery(slug, includeNavigation: true, increaseViewCount: true);
            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return NotFound(ApiResponse<ContentDetailDto>.ErrorResult("İçerik bulunamadı."));

            return Ok(ApiResponse<ContentDetailDto>.SuccessResult(result.Data));
        }

        /// <summary>
        /// Öne çıkan içerikleri getirir
        /// </summary>
        [HttpGet("featured")]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentListDto>>>> GetFeaturedContents(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? type = null)
        {
            var query = new GetContentsQuery
            {
                Page = page,
                PageSize = pageSize,
                Type = !string.IsNullOrEmpty(type) ? Enum.Parse<Domain.Enums.ContentType>(type, true) : null,
                Status = Domain.Enums.ContentStatus.Published,
                IsFeatured = true,
                SortBy = "CreatedDate",
                SortDirection = "desc"
            };

            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<PagedResult<ContentListDto>>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<PagedResult<ContentListDto>>.SuccessResult(result.Data));
        }

        /// <summary>
        /// Kategoriye göre içerikleri getirir
        /// </summary>
        [HttpGet("by-category/{categoryId:int}")]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentListDto>>>> GetContentsByCategory(
            int categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetContentsQuery
            {
                CategoryId = categoryId,
                Status = Domain.Enums.ContentStatus.Published,
                Page = page,
                PageSize = pageSize,
                SortBy = "CreatedDate",
                SortDirection = "desc"
            };

            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<PagedResult<ContentListDto>>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<PagedResult<ContentListDto>>.SuccessResult(result.Data));
        }

        /// <summary>
        /// En popüler içerikleri getirir (en çok görüntülenen)
        /// </summary>
        [HttpGet("popular")]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentListDto>>>> GetPopularContents(
            [FromQuery] int pageSize = 10,
            [FromQuery] string? type = null)
        {
            var query = new GetContentsQuery
            {
                Status = Domain.Enums.ContentStatus.Published,
                Type = !string.IsNullOrEmpty(type) ? Enum.Parse<Domain.Enums.ContentType>(type, true) : null,
                PageSize = pageSize,
                SortBy = "ViewCount",
                SortDirection = "desc"
            };

            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<PagedResult<ContentListDto>>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<PagedResult<ContentListDto>>.SuccessResult(result.Data));
        }

        /// <summary>
        /// En son eklenen içerikleri getirir
        /// </summary>
        [HttpGet("latest")]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentListDto>>>> GetLatestContents(
            [FromQuery] int pageSize = 10,
            [FromQuery] string? type = null)
        {
            var query = new GetContentsQuery
            {
                Status = Domain.Enums.ContentStatus.Published,
                Type = !string.IsNullOrEmpty(type) ? Enum.Parse<Domain.Enums.ContentType>(type, true) : null,
                PageSize = pageSize,
                SortBy = "CreatedDate",
                SortDirection = "desc"
            };

            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<PagedResult<ContentListDto>>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<PagedResult<ContentListDto>>.SuccessResult(result.Data));
        }

        /// <summary>
        /// İlgili içerikleri getirir
        /// </summary>
        [HttpGet("{contentId:int}/related")]
        public async Task<ActionResult<ApiResponse<List<ContentListDto>>>> GetRelatedContents(
            int contentId,
            [FromQuery] int count = 5)
        {
            var content = await _mediator.Send(new GetContentByIdQuery(contentId));

            if (!content.Succeeded)
                return NotFound(ApiResponse<List<ContentListDto>>.ErrorResult("İçerik bulunamadı."));

            var query = new GetContentsQuery
            {
                CategoryId = content.Data.CategoryId,
                Status = Domain.Enums.ContentStatus.Published,
                PageSize = count + 1,
                SortBy = "CreatedDate",
                SortDirection = "desc"
            };

            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<List<ContentListDto>>.ErrorResult(result.Errors.First()));

            var relatedContents = result.Data.Items.Where(c => c.Id != contentId).Take(count).ToList();

            return Ok(ApiResponse<List<ContentListDto>>.SuccessResult(relatedContents));
        }

        /// <summary>
        /// Yeni içerik oluşturur (Admin)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> CreateContent([FromBody] CreateContentCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<int>.ErrorResult(result.Errors.First()));

            return CreatedAtAction(
                nameof(GetContent),
                new { id = result.Data },
                ApiResponse<int>.SuccessResult(result.Data, "İçerik başarıyla oluşturuldu."));
        }

        /// <summary>
        /// İçeriği günceller (Admin)
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateContent(int id, [FromBody] UpdateContentCommand command)
        {
            if (id != command.Id)
                return BadRequest(ApiResponse<bool>.ErrorResult("ID uyuşmazlığı."));

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "İçerik başarıyla güncellendi."));
        }

        /// <summary>
        /// İçeriği siler (Admin)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteContent(int id)
        {
            var command = new DeleteContentCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "İçerik başarıyla silindi."));
        }

        /// <summary>
        /// İçerik slug'ının benzersiz olup olmadığını kontrol eder
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

        /// <summary>
        /// İçerik görüntülenme sayısını artırır
        /// </summary>
        [HttpPost("{id:int}/increment-view")]
        public async Task<ActionResult<ApiResponse<bool>>> IncrementViewCount(int id)
        {
            var query = new GetContentByIdQuery(id, increaseViewCount: true);
            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return NotFound(ApiResponse<bool>.ErrorResult("İçerik bulunamadı."));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Görüntülenme sayısı artırıldı."));
        }
    }
}
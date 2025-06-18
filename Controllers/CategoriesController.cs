using Application.Common.Models;
using Application.Features.Categories.Commands.Create;
using Application.Features.Categories.Commands.Delete;
using Application.Features.Categories.Commands.Update;
using Application.Features.Categories.DTOs;
using Application.Features.Categories.Queries.GetAll;
using Application.Features.Categories.Queries.GetById;
using Application.Features.Categories.Queries.GetBySlug;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WEB.Api.Controllers
{
    /// <summary>
    /// Kategori yönetimi için API endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tüm kategorileri sayfalı olarak getirir
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CategoryListDto>>>> GetCategories(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? parentId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = "SortOrder",
            [FromQuery] bool sortDescending = false)
        {
            var query = new GetCategoriesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                ParentId = parentId,
                IsActive = isActive,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<CategoryListDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Belirli bir kategoriyi ID ile getirir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<CategoryDetailDto>>> GetCategory(int id)
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(ApiResponse<CategoryDetailDto>.ErrorResult("Kategori bulunamadı."));

            return Ok(ApiResponse<CategoryDetailDto>.SuccessResult(result));
        }

        /// <summary>
        /// Belirli bir kategoriyi slug ile getirir
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        public async Task<ActionResult<ApiResponse<CategoryDetailDto>>> GetCategoryBySlug(string slug)
        {
            var query = new GetCategoryBySlugQuery(slug);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(ApiResponse<CategoryDetailDto>.ErrorResult("Kategori bulunamadı."));

            return Ok(ApiResponse<CategoryDetailDto>.SuccessResult(result));
        }

        /// <summary>
        /// Hiyerarşik kategori ağacını getirir (sadece aktif olanlar)
        /// </summary>
        [HttpGet("hierarchy")]
        public async Task<ActionResult<ApiResponse<PagedResult<CategoryListDto>>>> GetCategoryHierarchy()
        {
            var query = new GetCategoriesQuery
            {
                ParentId = null,
                IsActive = true,
                PageSize = 1000,
                SortBy = "SortOrder"
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<CategoryListDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Alt kategorileri getirir
        /// </summary>
        [HttpGet("{parentId:int}/children")]
        public async Task<ActionResult<ApiResponse<PagedResult<CategoryListDto>>>> GetChildCategories(int parentId)
        {
            var query = new GetCategoriesQuery
            {
                ParentId = parentId,
                IsActive = true,
                PageSize = 100,
                SortBy = "SortOrder"
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<CategoryListDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Yeni kategori oluşturur (Admin)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<int>.ErrorResult(result.Errors.First()));

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = result.Data },
                ApiResponse<int>.SuccessResult(result.Data, "Kategori başarıyla oluşturuldu."));
        }

        /// <summary>
        /// Kategoriyi günceller (Admin)
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCategory(int id, [FromBody] UpdateCategoryCommand command)
        {
            if (id != command.Id)
                return BadRequest(ApiResponse<bool>.ErrorResult("ID uyuşmazlığı."));

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Kategori başarıyla güncellendi."));
        }

        /// <summary>
        /// Kategoriyi siler (Admin)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Kategori başarıyla silindi."));
        }

        /// <summary>
        /// Kategori slug'ının benzersiz olup olmadığını kontrol eder
        /// </summary>
        [HttpGet("check-slug")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckSlugAvailability(
            [FromQuery, Required] string slug,
            [FromQuery] int? excludeId = null)
        {
            var existingCategory = await _mediator.Send(new GetCategoryBySlugQuery(slug));

            bool isAvailable = existingCategory == null || (excludeId.HasValue && existingCategory.Id == excludeId.Value);

            return Ok(ApiResponse<bool>.SuccessResult(isAvailable,
                isAvailable ? "Slug kullanılabilir." : "Bu slug zaten kullanımda."));
        }
    }
}
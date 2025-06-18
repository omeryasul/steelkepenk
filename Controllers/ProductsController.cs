    using Application.Common.Models;
    using Application.Features.Products.Commands.Create;
    using Application.Features.Products.Commands.Delete;
    using Application.Features.Products.Commands.Update;
    using Application.Features.Products.DTOs;
    using Application.Features.Products.Queries.GetAll;
    using Application.Features.Products.Queries.GetByCategory;
    using Application.Features.Products.Queries.GetById;
    using Application.Features.Products.Queries.GetBySlug;
    using Application.Features.Products.ToggleFeatured;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.ComponentModel.DataAnnotations;

    namespace WEB.Api.Controllers
    {
        /// <summary>
        /// Ürün yönetimi için API endpoints
        /// </summary>
        [ApiController]
        [Route("api/[controller]")]
        [Produces("application/json")]
        public class ProductsController : ControllerBase
        {
            private readonly IMediator _mediator;

            public ProductsController(IMediator mediator)
            {
                _mediator = mediator;
            }

            /// <summary>
            /// Tüm ürünleri sayfalı olarak getirir
            /// </summary>
            [HttpGet]
            public async Task<ActionResult<ApiResponse<PagedResult<ProductListDto>>>> GetProducts(
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 12,
                [FromQuery] string? searchTerm = null,
                [FromQuery] int? categoryId = null,
                [FromQuery] int? status = null,
                [FromQuery] bool? isFeatured = null,
                [FromQuery] string? sortBy = "SortOrder",
                [FromQuery] bool sortDescending = false)
            {
                var query = new GetProductsQuery
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    CategoryId = categoryId,
                    Status = status,
                    IsFeatured = isFeatured,
                    SortBy = sortBy,
                    SortDescending = sortDescending
                };

                var result = await _mediator.Send(query);

                if (!result.Succeeded)
                    return BadRequest(ApiResponse<PagedResult<ProductListDto>>.ErrorResult(result.Errors.First()));

                return Ok(ApiResponse<PagedResult<ProductListDto>>.SuccessResult(result.Data));
            }

            /// <summary>
            /// Belirli bir ürünü ID ile getirir
            /// </summary>
            [HttpGet("{id:int}")]
            public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetProduct(int id)
            {
                var query = new GetProductByIdQuery(id);
                var result = await _mediator.Send(query);

                if (!result.Succeeded)
                    return NotFound(ApiResponse<ProductDetailDto>.ErrorResult("Ürün bulunamadı."));

                return Ok(ApiResponse<ProductDetailDto>.SuccessResult(result.Data));
            }

            /// <summary>
            /// Belirli bir ürünü slug ile getirir
            /// </summary>
            [HttpGet("by-slug/{slug}")]
            public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetProductBySlug(string slug)
            {
                var query = new GetProductBySlugQuery(slug);
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFound(ApiResponse<ProductDetailDto>.ErrorResult("Ürün bulunamadı."));

                return Ok(ApiResponse<ProductDetailDto>.SuccessResult(result));
            }

            /// <summary>
            /// Öne çıkan ürünleri getirir
            /// </summary>
            [HttpGet("featured")]
            public async Task<ActionResult<ApiResponse<List<ProductListDto>>>> GetFeaturedProducts([FromQuery] int take = 8)
            {
                var query = new GetFeaturedProductsQuery(take);
                var result = await _mediator.Send(query);

                return Ok(ApiResponse<List<ProductListDto>>.SuccessResult(result));
            }

            /// <summary>
            /// Kategoriye göre ürünleri getirir
            /// </summary>
            [HttpGet("by-category/{categoryId:int}")]
            public async Task<ActionResult<ApiResponse<PagedResult<ProductListDto>>>> GetProductsByCategory(
                int categoryId,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 12,
                [FromQuery] string? sortBy = "SortOrder",
                [FromQuery] bool sortDescending = false)
            {
                var query = new GetProductsByCategoryQuery
                {
                    CategoryId = categoryId,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortDescending = sortDescending
                };

                var result = await _mediator.Send(query);
                return Ok(ApiResponse<PagedResult<ProductListDto>>.SuccessResult(result));
            }

            /// <summary>
            /// En popüler ürünleri getirir (en çok görüntülenen)
            /// </summary>
            [HttpGet("popular")]
            public async Task<ActionResult<ApiResponse<PagedResult<ProductListDto>>>> GetPopularProducts([FromQuery] int count = 10)
            {
                var query = new GetProductsQuery
                {
                    PageSize = count,
                    SortBy = "ViewCount",
                    SortDescending = true
                };

                var result = await _mediator.Send(query);

                if (!result.Succeeded)
                    return BadRequest(ApiResponse<PagedResult<ProductListDto>>.ErrorResult(result.Errors.First()));

                return Ok(ApiResponse<PagedResult<ProductListDto>>.SuccessResult(result.Data));
            }

            /// <summary>
            /// En son eklenen ürünleri getirir
            /// </summary>
            [HttpGet("latest")]
            public async Task<ActionResult<ApiResponse<PagedResult<ProductListDto>>>> GetLatestProducts([FromQuery] int count = 10)
            {
                var query = new GetProductsQuery
                {
                    PageSize = count,
                    SortBy = "CreatedDate",
                    SortDescending = true
                };

                var result = await _mediator.Send(query);

                if (!result.Succeeded)
                    return BadRequest(ApiResponse<PagedResult<ProductListDto>>.ErrorResult(result.Errors.First()));

                return Ok(ApiResponse<PagedResult<ProductListDto>>.SuccessResult(result.Data));
            }

            /// <summary>
            /// İlgili ürünleri getirir
            /// </summary>
            [HttpGet("{productId:int}/related")]
            public async Task<ActionResult<ApiResponse<List<ProductListDto>>>> GetRelatedProducts(
                int productId,
                [FromQuery] int count = 6)
            {
                var product = await _mediator.Send(new GetProductByIdQuery(productId));

                if (!product.Succeeded)
                    return NotFound(ApiResponse<List<ProductListDto>>.ErrorResult("Ürün bulunamadı."));

                var query = new GetProductsByCategoryQuery
                {
                    CategoryId = product.Data.CategoryId,
                    PageSize = count + 1
                };

                var result = await _mediator.Send(query);
                var relatedProducts = result.Items.Where(p => p.Id != productId).Take(count).ToList();

                return Ok(ApiResponse<List<ProductListDto>>.SuccessResult(relatedProducts));
            }

            /// <summary>
            /// Yeni ürün oluşturur (Admin)
            /// </summary>
            [HttpPost]
            [Authorize]
            public async Task<ActionResult<ApiResponse<int>>> CreateProduct([FromBody] CreateProductCommand command)
            {
                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                    return BadRequest(ApiResponse<int>.ErrorResult(result.Errors.First()));

                return CreatedAtAction(
                    nameof(GetProduct),
                    new { id = result.Data },
                    ApiResponse<int>.SuccessResult(result.Data, "Ürün başarıyla oluşturuldu."));
            }

            /// <summary>
            /// Ürünü günceller (Admin)
            /// </summary>
            [HttpPut("{id:int}")]
            [Authorize]
            public async Task<ActionResult<ApiResponse<bool>>> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
            {
                if (id != command.Id)
                    return BadRequest(ApiResponse<bool>.ErrorResult("ID uyuşmazlığı."));

                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                    return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

                return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Ürün başarıyla güncellendi."));
            }

            /// <summary>
            /// Ürün durumunu günceller (Admin)
            /// </summary>
            [HttpPut("{id:int}/status")]
            [Authorize]
            public async Task<ActionResult<ApiResponse<bool>>> UpdateProductStatus(int id, [FromBody] UpdateProductStatusCommand command)
            {
                if (id != command.Id)
                    return BadRequest(ApiResponse<bool>.ErrorResult("ID uyuşmazlığı."));

                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                    return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

                return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Ürün durumu başarıyla güncellendi."));
            }

            /// <summary>
            /// Ürünün öne çıkarma durumunu değiştirir (Admin)
            /// </summary>
            [HttpPost("{id:int}/toggle-featured")]
            [Authorize]
            public async Task<ActionResult<ApiResponse<bool>>> ToggleProductFeatured(int id)
            {
                var command = new ToggleProductFeaturedCommand(id);
                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                    return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

                return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Ürün öne çıkarma durumu başarıyla değiştirildi."));
            }

            /// <summary>
            /// Ürünü siler (Admin)
            /// </summary>
            [HttpDelete("{id:int}")]
            [Authorize]
            public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(int id)
            {
                var command = new DeleteProductCommand(id);
                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                    return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

                return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Ürün başarıyla silindi."));
            }

            /// <summary>
            /// Ürün slug'ının benzersiz olup olmadığını kontrol eder
            /// </summary>
            [HttpGet("check-slug")]
            public async Task<ActionResult<ApiResponse<bool>>> CheckSlugAvailability(
                [FromQuery, Required] string slug,
                [FromQuery] int? excludeId = null)
            {
                var existingProduct = await _mediator.Send(new GetProductBySlugQuery(slug));

                bool isAvailable = existingProduct == null || (excludeId.HasValue && existingProduct.Id == excludeId.Value);

                return Ok(ApiResponse<bool>.SuccessResult(isAvailable,
                    isAvailable ? "Slug kullanılabilir." : "Bu slug zaten kullanımda."));
            }
        }
    }
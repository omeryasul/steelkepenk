using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IMemoryCache _cache;

    public ProductController(ILogger<ProductController> logger, IMemoryCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    [HttpPost("related")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetRelatedProducts([FromBody] RelatedProductsRequest request)
    {
        try
        {
            // Input validation
            if (request == null || request.CategoryId <= 0)
            {
                return BadRequest("Geçersiz istek.");
            }

            // Rate limiting kontrolü
            var clientIp = GetClientIpAddress();
            var rateLimitKey = $"related_products_{clientIp}";

            if (_cache.TryGetValue(rateLimitKey, out int requestCount))
            {
                if (requestCount >= 10) // Dakikada maksimum 10 istek
                {
                    return StatusCode(429, "Rate limit exceeded");
                }
                _cache.Set(rateLimitKey, requestCount + 1, TimeSpan.FromMinutes(1));
            }
            else
            {
                _cache.Set(rateLimitKey, 1, TimeSpan.FromMinutes(1));
            }

            // Cache key oluştur
            var cacheKey = $"related_products_{request.CategoryId}_{request.ExcludeId}_{request.Count}";

            if (_cache.TryGetValue(cacheKey, out List<ProductDto> cachedProducts))
            {
                return Ok(cachedProducts);
            }

            // Veri yükle (güvenli şekilde)
            var products = await LoadRelatedProductsAsync(request);

            // XSS koruması: Response data'sını sanitize et
            foreach (var product in products)
            {
                product.Name = System.Web.HttpUtility.HtmlEncode(product.Name);
                product.ShortDescription = System.Web.HttpUtility.HtmlEncode(product.ShortDescription);
                product.CategoryName = System.Web.HttpUtility.HtmlEncode(product.CategoryName);
                product.Slug = SanitizeSlug(product.Slug);
                product.MainImage = SanitizeImageUrl(product.MainImage);
            }

            // Cache'e kaydet
            _cache.Set(cacheKey, products, TimeSpan.FromMinutes(15));

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading related products. CategoryId: {CategoryId}", request?.CategoryId);
            return StatusCode(500, "Bir hata oluştu.");
        }
    }

    private async Task<List<ProductDto>> LoadRelatedProductsAsync(RelatedProductsRequest request)
    {
        // Database'den güvenli veri yükleme implementasyonu
        // Parametreli sorgu kullanılmalı (SQL injection koruması)
        return new List<ProductDto>();
    }

    private string GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private string SanitizeSlug(string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return string.Empty;

        // Sadece harf, rakam ve tire karakterlerine izin ver
        return System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-zA-Z0-9\-]", "");
    }

    private string SanitizeImageUrl(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return "/images/product-placeholder.jpg";

        // Sadece güvenli URL'lere izin ver
        if (imageUrl.StartsWith("/images/") ||
            imageUrl.StartsWith("https://") && Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
        {
            return imageUrl;
        }

        return "/images/product-placeholder.jpg";
    }
}

// DTO Classes
public class RelatedProductsRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
    public int CategoryId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "ExcludeId must be non-negative")]
    public int ExcludeId { get; set; }

    [Range(1, 20, ErrorMessage = "Count must be between 1 and 20")]
    public int Count { get; set; } = 4;
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MainImage { get; set; } = string.Empty;
}
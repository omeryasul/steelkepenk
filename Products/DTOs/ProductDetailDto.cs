using Application.Features.Categories.DTOs;
using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.Features.Products.DTOs
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; } = 0;
        public decimal? DiscountPrice { get; set; } = null;
        public string? FeaturedImage { get; set; }
        public string? MainImage { get; set; }
        public List<string> Images { get; set; } = new();
        public ProductStatus Status { get; set; }
        public bool IsFeatured { get; set; }
        public int ViewCount { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }

        // Product Details
        public string? Features { get; set; }
        public string? Specifications { get; set; }
        public string? UsageAreas { get; set; }
        public string? Advantages { get; set; }
        public string? DisplayPrice { get; set; }
        public string? PriceNote { get; set; }

        // Category bilgileri
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategorySlug { get; set; }
        public CategoryDto? Category { get; set; } // Bu property eklendi

        // SEO bilgileri
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string? MetaKeywords { get; set; }

        // Open Graph bilgileri
        public string? OgTitle { get; set; }
        public string? OgDescription { get; set; }
        public string? OgImage { get; set; }

        // Tags
        public List<ProductTagDto> Tags { get; set; } = new();

        // Navigation
        public ProductNavigationDto? PreviousProduct { get; set; }
        public ProductNavigationDto? NextProduct { get; set; }
    }

    public class ProductTagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }

    public class ProductNavigationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }
}
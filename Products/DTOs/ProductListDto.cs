using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.Features.Products.DTOs
{
    public class ProductListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal? DiscountPrice { get; set; }
        public string? FeaturedImage { get; set; }
        public string? DisplayPrice { get; set; }
        public string? MainImage { get; set; }
        public string? PriceNote { get; set; }
        public ProductStatus Status { get; set; }
        public bool IsFeatured { get; set; }
        public int ViewCount { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int TotalCount { get; set; }

        // Category bilgileri
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategorySlug { get; set; }

        // Tags
        public List<string> Tags { get; set; } = new();
    }
}
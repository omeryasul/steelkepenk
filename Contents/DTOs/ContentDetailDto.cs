using Application.Features.Categories.DTOs;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.DTOs
{
    public class ContentDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? FeaturedImage { get; set; }
        public DateTime CreatedDate { get; set; }  // CreatedDate yerine  
        public DateTime? UpdatedDate { get; set; } // UpdatedDate yerine
        public ContentStatus Status { get; set; }
        public ContentType Type { get; set; }
        public int ViewCount { get; set; }
        public bool IsFeatured { get; set; }
        public int SortOrder { get; set; }

        // Category bilgileri
        // Category bilgileri
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategorySlug { get; set; }
        public CategoryDto? Category { get; set; } // Bu property eklendi

        // SEO bilgileri
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string? MetaKeywords { get; set; }
        public string? OgTitle { get; set; }
        public string? OgDescription { get; set; }
        public string? OgImage { get; set; }

        // Tags
        public List<ContentTagDto> Tags { get; set; } = new();

        // Navigation
        public ContentNavigationDto? PreviousContent { get; set; }
        public ContentNavigationDto? NextContent { get; set; }
    }

    public class ContentTagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }

    public class ContentNavigationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }
}

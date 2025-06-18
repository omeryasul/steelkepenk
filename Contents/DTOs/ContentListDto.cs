using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.DTOs
{
    public class ContentListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? FeaturedImage { get; set; }

        // Entity'deki property isimleriyle uyumlu
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public ContentStatus Status { get; set; }
        public ContentType Type { get; set; }
        public int ViewCount { get; set; }
        public bool IsFeatured { get; set; }
        public int SortOrder { get; set; }

        // Category bilgileri
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategorySlug { get; set; }

        // SEO bilgileri
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;

        // Tags
        public List<string> Tags { get; set; } = new();
    }
}
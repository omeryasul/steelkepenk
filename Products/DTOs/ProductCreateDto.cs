using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.DTOs
{
    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string Description { get; set; } = string.Empty;
        public ProductStatus Status { get; set; } = ProductStatus.Draft;
        public bool IsFeatured { get; set; } = false;
        public int SortOrder { get; set; } = 0;
        public int CategoryId { get; set; }

        // Ürün Detayları
        public string? Features { get; set; }
        public string? Specifications { get; set; }
        public string? UsageAreas { get; set; }
        public string? Advantages { get; set; }

        // Fiyat
        public decimal? DisplayPrice { get; set; }
        public string? PriceNote { get; set; }

        // SEO
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string? MetaKeywords { get; set; }
        public string? OgTitle { get; set; }
        public string? OgDescription { get; set; }
    }
}

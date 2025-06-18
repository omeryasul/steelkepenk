using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.DTOs
{
    public class ProductUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string Description { get; set; } = string.Empty;
        public ProductStatus Status { get; set; }
        public bool IsFeatured { get; set; }
        public int SortOrder { get; set; }
        public int CategoryId { get; set; }
        public string? Features { get; set; }
        public string? Specifications { get; set; }
        public string? UsageAreas { get; set; }
        public string? Advantages { get; set; }
        public decimal? DisplayPrice { get; set; }
        public string? PriceNote { get; set; }
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string? MetaKeywords { get; set; }
        public string? OgTitle { get; set; }
        public string? OgDescription { get; set; }
    }
}

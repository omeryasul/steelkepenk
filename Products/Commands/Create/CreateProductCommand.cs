using Application.Common.Models;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands.Create
{
    public record CreateProductCommand : IRequest<Result<int>>
    {
        public string Name { get; init; } = string.Empty;
        public string? ShortDescription { get; init; }
        public string Description { get; init; } = string.Empty;
        public ProductStatus Status { get; init; } = ProductStatus.Draft;
        public bool IsFeatured { get; init; } = false;
        public int SortOrder { get; init; } = 0;
        public int CategoryId { get; init; }
        public string? MainImage { get; set; }

        // Ürün Detayları
        public string? Features { get; init; }
        public string? Specifications { get; init; }
        public string? UsageAreas { get; init; }
        public string? Advantages { get; init; }

        // Fiyat
        public decimal? DisplayPrice { get; init; }
        public string? PriceNote { get; init; }

        // SEO
        public string MetaTitle { get; init; } = string.Empty;
        public string MetaDescription { get; init; } = string.Empty;
        public string? MetaKeywords { get; init; }
        public string? OgTitle { get; init; }
        public string? OgDescription { get; init; }

        // Tags
        public List<int> TagIds { get; init; } = new();
    }
}

using Application.Common.Models;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Commands.Create
{
    public record CreateContentCommand : IRequest<Result<int>>
    {
        public string Title { get; init; } = string.Empty;
        public string? Slug { get; init; }
        public string Summary { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
        public string? FeaturedImage { get; init; }
        public ContentType Type { get; init; }
        public ContentStatus Status { get; init; } = ContentStatus.Draft;
        public int CategoryId { get; init; }
        public bool IsFeatured { get; init; }
        public int SortOrder { get; init; }

        // SEO Fields
        public string MetaTitle { get; init; } = string.Empty;
        public string MetaDescription { get; init; } = string.Empty;
        public string? MetaKeywords { get; init; }
        public string? OgTitle { get; init; }
        public string? OgDescription { get; init; }
        public string? OgImage { get; init; }
        public string? CanonicalUrl { get; init; }
        public bool NoIndex { get; init; } = false;
        public bool NoFollow { get; init; } = false;

        // Date fields
        public DateTime? PublishedDate { get; init; }

        // Tags
        public List<string> Tags { get; init; } = new();
    }
}
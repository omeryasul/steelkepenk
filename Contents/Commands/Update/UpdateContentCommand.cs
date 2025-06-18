using Application.Common.Models;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Commands.Update
{
    public class UpdateContentCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public ContentType Type { get; set; }
        public ContentStatus Status { get; set; }
        public int? CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        public int SortOrder { get; set; }
        public DateTime? PublishedDate { get; set; }

        // SEO Fields
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? CanonicalUrl { get; set; }
        public bool? NoIndex { get; set; }
        public bool? NoFollow { get; set; }

        // Tags
        public List<string> Tags { get; set; } = new();
    }
}

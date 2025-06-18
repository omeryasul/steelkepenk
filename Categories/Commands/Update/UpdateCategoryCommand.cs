using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Commands.Update
{
    public record UpdateCategoryCommand : IRequest<Result<bool>>
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int? ParentId { get; init; }
        public int SortOrder { get; init; } = 0;
        public bool IsActive { get; init; } = true;

        // SEO
        public string MetaTitle { get; init; } = string.Empty;
        public string MetaDescription { get; init; } = string.Empty;
        public string? MetaKeywords { get; init; }
    }
}

using Application.Common.Models;
using Application.Features.Categories.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Queries.GetAll
{
    public record GetCategoriesQuery : IRequest<PagedResult<CategoryListDto>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? SearchTerm { get; init; }
        public int? ParentId { get; init; }
        public bool? IsActive { get; init; }
        public string? SortBy { get; init; } = "SortOrder";
        public bool SortDescending { get; init; } = false;
    }
}

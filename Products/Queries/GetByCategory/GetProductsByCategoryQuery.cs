using Application.Common.Models;
using Application.Features.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Queries.GetByCategory
{
    public record GetProductsByCategoryQuery : IRequest<PagedResult<ProductListDto>>
    {
        public int CategoryId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 12;
        public string? SortBy { get; init; } = "SortOrder";
        public bool SortDescending { get; init; } = false;
    }
}

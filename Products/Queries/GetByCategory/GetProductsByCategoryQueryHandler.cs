using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Products.DTOs;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Queries.GetByCategory
{
    public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, PagedResult<ProductListDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetProductsByCategoryQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductListDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Products
                .Include(x => x.Category)
                .Where(x => x.CategoryId == request.CategoryId && x.Status == ProductStatus.Active);

            // Sorting
            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "createdat" => request.SortDescending ? query.OrderByDescending(x => x.CreatedDate) : query.OrderBy(x => x.CreatedDate),
                "viewcount" => request.SortDescending ? query.OrderByDescending(x => x.ViewCount) : query.OrderBy(x => x.ViewCount),
                "price" => request.SortDescending ? query.OrderByDescending(x => x.DisplayPrice) : query.OrderBy(x => x.DisplayPrice),
                _ => request.SortDescending ? query.OrderByDescending(x => x.SortOrder) : query.OrderBy(x => x.SortOrder)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var products = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductListDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    ShortDescription = x.ShortDescription,
                    MainImage = x.MainImage,
                    Status = x.Status,
                    IsFeatured = x.IsFeatured,
                    SortOrder = x.SortOrder,
                    ViewCount = x.ViewCount,
                    PriceNote = x.PriceNote,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync(cancellationToken);

            return PagedResult<ProductListDto>.Create(products, totalCount, request.PageNumber, request.PageSize);
        }
    }
}

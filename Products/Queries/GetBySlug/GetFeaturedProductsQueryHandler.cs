using Application.Common.Interfaces;
using Application.Features.Products.DTOs;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Queries.GetBySlug
{
    public class GetFeaturedProductsQueryHandler : IRequestHandler<GetFeaturedProductsQuery, List<ProductListDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetFeaturedProductsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductListDto>> Handle(GetFeaturedProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _context.Products
                .Include(x => x.Category)
                .Where(x => x.IsFeatured && x.Status == ProductStatus.Active)
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedDate)
                .Take(request.Take)
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

            return products;
        }
    }
}

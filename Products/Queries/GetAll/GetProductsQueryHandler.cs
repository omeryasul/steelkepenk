using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Queries.GetAll
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductListDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetProductsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<ProductListDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Products
                    .Include(x => x.Category)
                    .AsQueryable();

                // Filters
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(x => x.Name.Contains(request.SearchTerm) ||
                                           x.Description.Contains(request.SearchTerm) ||
                                           x.ShortDescription!.Contains(request.SearchTerm));
                }

                if (request.CategoryId.HasValue)
                {
                    query = query.Where(x => x.CategoryId == request.CategoryId.Value);
                }

                if (request.Status.HasValue)
                {
                    query = query.Where(x => (int)x.Status == request.Status.Value);
                }

                if (request.IsFeatured.HasValue)
                {
                    query = query.Where(x => x.IsFeatured == request.IsFeatured.Value);
                }

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

                var pagedResult = PagedResult<ProductListDto>.Create(products, totalCount, request.PageNumber, request.PageSize);
                return Result<PagedResult<ProductListDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<ProductListDto>>.Failure($"Ürünler getirilirken hata oluştu: {ex.Message}");
            }
        }
    }
}
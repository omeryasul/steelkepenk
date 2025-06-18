using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Categories.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Queries.GetAll
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryListDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCategoriesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<CategoryListDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Categories
                .Include(x => x.Parent)
                .Include(x => x.Contents)
                .AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x => x.Name.Contains(request.SearchTerm) ||
                                       x.Description!.Contains(request.SearchTerm));
            }

            if (request.ParentId.HasValue)
            {
                query = query.Where(x => x.ParentId == request.ParentId.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            // Sorting
            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "createdat" => request.SortDescending ? query.OrderByDescending(x => x.CreatedDate) : query.OrderBy(x => x.CreatedDate),
                _ => request.SortDescending ? query.OrderByDescending(x => x.SortOrder) : query.OrderBy(x => x.SortOrder)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var categories = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new CategoryListDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    Description = x.Description,
                    Image = x.Image,
                    ParentId = x.ParentId,
                    ParentName = x.Parent != null ? x.Parent.Name : null,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive,
                    ContentCount = x.Contents.Count,
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync(cancellationToken);

            return PagedResult<CategoryListDto>.Create(categories, totalCount, request.PageNumber, request.PageSize);
        }
    }
}

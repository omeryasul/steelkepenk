using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Tags.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tags.Queries.GetAll
{
    public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, PagedResult<TagDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetTagsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            // Validate request
            if (request.PageNumber < 1 || request.PageSize < 1 || request.PageSize > 100)
            {
                return PagedResult<TagDto>.Failure("Invalid pagination parameters");
            }

            var query = _context.Tags.AsNoTracking().AsQueryable();

            // Sanitize and trim search term
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                if (searchTerm.Length > 100) // Add reasonable limit
                {
                    searchTerm = searchTerm.Substring(0, 100);
                }

                query = query.Where(x => x.Name.Contains(searchTerm) ||
                                       (x.Description != null && x.Description.Contains(searchTerm)));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            // Define allowed sort fields
            var allowedSortFields = new[] { "name", "createdat" };
            var sortBy = request.SortBy?.ToLower();
            if (!string.IsNullOrEmpty(sortBy) && !allowedSortFields.Contains(sortBy))
            {
                sortBy = "name"; // Default to name if invalid
            }

            query = sortBy switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "createdat" => request.SortDescending ? query.OrderByDescending(x => x.CreatedDate) : query.OrderBy(x => x.CreatedDate),
                _ => query.OrderBy(x => x.Name)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var tags = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new TagDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    Description = x.Description,
                    Color = x.Color,
                    IsActive = x.IsActive,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            return PagedResult<TagDto>.Create(tags, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
using Application.Common.Interfaces;
using Application.Features.Tags.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tags.Queries.GetBySlug
{
    public class GetTagBySlugQueryHandler : IRequestHandler<GetTagBySlugQuery, TagDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetTagBySlugQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TagDto?> Handle(GetTagBySlugQuery request, CancellationToken cancellationToken)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(x => x.Slug == request.Slug && x.IsActive, cancellationToken);

            if (tag == null)
                return null;

            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug,
                Description = tag.Description,
                Color = tag.Color,
                IsActive = tag.IsActive,
                CreatedDate = tag.CreatedDate,
                UpdatedDate = tag.UpdatedDate
            };
        }
    }
}
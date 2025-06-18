using Application.Common.Interfaces;
using Application.Features.Tags.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tags.Queries.GetById
{
    public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetTagByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TagDto?> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

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
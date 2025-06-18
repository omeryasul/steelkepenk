using Application.Common.Interfaces;
using Application.Features.Categories.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Queries.GetBySlug
{
    public class GetCategoryBySlugQueryHandler : IRequestHandler<GetCategoryBySlugQuery, CategoryDetailDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetCategoryBySlugQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryDetailDto?> Handle(GetCategoryBySlugQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .Include(x => x.Parent)
                .Include(x => x.Children.Where(c => c.IsActive))
                .Include(x => x.Contents)
                .FirstOrDefaultAsync(x => x.Slug == request.Slug && x.IsActive, cancellationToken);

            if (category == null)
                return null;

            return new CategoryDetailDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                Image = category.Image,
                ParentId = category.ParentId,
                ParentName = category.Parent?.Name,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive,
                MetaTitle = category.MetaTitle,
                MetaDescription = category.MetaDescription,
                MetaKeywords = category.MetaKeywords,
                Children = category.Children.Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentId = c.ParentId,
                    SortOrder = c.SortOrder,
                    IsActive = c.IsActive,
                    ContentCount = c.Contents.Count,
                    CreatedDate = c.CreatedDate
                }).ToList(),
                ContentCount = category.Contents.Count,
                CreatedDate = category.CreatedDate,
                UpdatedDate = category.UpdatedDate,
                CreatedBy = category.CreatedBy
            };
        }
    }
}

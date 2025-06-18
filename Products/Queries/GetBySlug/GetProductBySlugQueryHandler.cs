using Application.Common.Interfaces;
using Application.Features.Categories.DTOs;
using Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Queries.GetBySlug
{
    public class GetProductBySlugQueryHandler : IRequestHandler<GetProductBySlugQuery, ProductDetailDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetProductBySlugQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDetailDto?> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages.OrderBy(i => i.SortOrder))
                .Include(x => x.ProductTags)
                    .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(x => x.Slug == request.Slug, cancellationToken);

            if (product == null)
                return null;

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                ShortDescription = product.ShortDescription,
                Description = product.Description,

                MainImage = product.MainImage,
                FeaturedImage = product.MainImage,
                Status = product.Status,
                IsFeatured = product.IsFeatured,
                SortOrder = product.SortOrder,
                ViewCount = product.ViewCount,
                Features = product.Features,
                Specifications = product.Specifications,
                UsageAreas = product.UsageAreas,
                Advantages = product.Advantages,
                PriceNote = product.PriceNote,
                MetaTitle = product.MetaTitle,
                MetaDescription = product.MetaDescription,
                MetaKeywords = product.MetaKeywords,
                OgTitle = product.OgTitle,
                OgDescription = product.OgDescription,
                OgImage = product.OgImage,
                CategoryId = product.Category.Id,
                CategoryName = product.Category.Name,
                CategorySlug = product.Category.Slug,
                Category = new CategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Slug = product.Category.Slug
                },
                Images = product.ProductImages.Select(i => i.ImageUrl).ToList(),
                Tags = product.ProductTags.Select(pt => new ProductTagDto
                {
                    Id = pt.Tag.Id,
                    Name = pt.Tag.Name,
                    Slug = pt.Tag.Slug
                }).ToList(),
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
                CreatedBy = product.CreatedBy
            };
        }
    }
}
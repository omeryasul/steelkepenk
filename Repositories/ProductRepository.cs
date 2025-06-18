using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(x => x.Category)
                .Include(x => x.ProductImages.OrderBy(pi => pi.SortOrder))
                .Include(x => x.ProductTags)
                    .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync(int? categoryId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .Include(x => x.Category)
                .Include(x => x.ProductImages.Where(pi => pi.IsMain))
                .Where(x => x.Status == ProductStatus.Active);

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count = 8, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(x => x.Category)
                .Include(x => x.ProductImages.Where(pi => pi.IsMain))
                .Where(x => x.Status == ProductStatus.Active && x.IsFeatured)
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedDate)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(x => x.Category)
                .Include(x => x.ProductImages.Where(pi => pi.IsMain))
                .Where(x => x.CategoryId == categoryId && x.Status == ProductStatus.Active)
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByTagAsync(int tagId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(x => x.Category)
                .Include(x => x.ProductImages.Where(pi => pi.IsMain))
                .Include(x => x.ProductTags)
                .Where(x => x.ProductTags.Any(pt => pt.TagId == tagId) && x.Status == ProductStatus.Active)
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsSlugUniqueAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(x => x.Slug == slug);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task IncrementViewCountAsync(int productId, CancellationToken cancellationToken = default)
        {
            var product = await _dbSet.FindAsync(new object[] { productId }, cancellationToken);
            if (product != null)
            {
                product.ViewCount++;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

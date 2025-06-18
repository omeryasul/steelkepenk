using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(x => x.Parent)
                .Include(x => x.Children.Where(c => c.IsActive))
                .Include(x => x.Contents)
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(x => x.Children.Where(c => c.IsActive))
                .Where(x => x.ParentId == null && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(x => x.ParentId == parentId && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
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

        public async Task<bool> HasChildrenAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(x => x.ParentId == categoryId, cancellationToken);
        }

        public async Task<bool> HasContentsAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Contents.AnyAsync(x => x.CategoryId == categoryId, cancellationToken);
        }
    }
}

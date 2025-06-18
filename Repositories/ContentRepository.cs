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
    public class ContentRepository : Repository<Content>, IContentRepository
    {
        public ContentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Content?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(x => x.Category)
                .Include(x => x.ContentTags)
                    .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
        }

        public async Task<IEnumerable<Content>> GetPublishedContentsAsync(int? categoryId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .Include(x => x.Category)
                .Where(x => x.Status == ContentStatus.Published);

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Content>> GetFeaturedContentsAsync(int count = 5, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(x => x.Category)
                .Where(x => x.Status == ContentStatus.Published && x.IsFeatured)
                .OrderByDescending(x => x.CreatedDate)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Content>> GetContentsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(x => x.Category)
                .Where(x => x.CategoryId == categoryId && x.Status == ContentStatus.Published)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Content>> GetContentsByTagAsync(int tagId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(x => x.Category)
                .Include(x => x.ContentTags)
                .Where(x => x.ContentTags.Any(ct => ct.TagId == tagId) && x.Status == ContentStatus.Published)
                .OrderByDescending(x => x.CreatedDate)
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
    }
}

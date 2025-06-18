using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public interface IContentRepository : IRepository<Content>
    {
        Task<Content?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<Content>> GetPublishedContentsAsync(int? categoryId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Content>> GetFeaturedContentsAsync(int count = 5, CancellationToken cancellationToken = default);
        Task<IEnumerable<Content>> GetContentsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Content>> GetContentsByTagAsync(int tagId, CancellationToken cancellationToken = default);
        Task<bool> IsSlugUniqueAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}

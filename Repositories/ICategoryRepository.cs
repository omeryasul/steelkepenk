using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId, CancellationToken cancellationToken = default);
        Task<bool> IsSlugUniqueAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> HasChildrenAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<bool> HasContentsAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}

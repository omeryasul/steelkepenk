using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetActiveProductsAsync(int? categoryId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count = 8, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByTagAsync(int tagId, CancellationToken cancellationToken = default);
        Task<bool> IsSlugUniqueAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
        Task IncrementViewCountAsync(int productId, CancellationToken cancellationToken = default);
    }
}

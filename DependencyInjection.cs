using Application.Common.Interfaces;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using Persistence.Services;

namespace Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Register DbContext as IApplicationDbContext
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            // Generic Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Specific Repositories
            services.AddScoped<IContentRepository, ContentRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            // Services
            services.AddScoped<ISlugService, SlugService>();
            services.AddScoped<ISeoService, SeoService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IBreadcrumbService, BreadcrumbService>(); // BU SATIRI SİLİN

            return services;
        }
    }
}
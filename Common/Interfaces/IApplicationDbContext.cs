// Core/Application/Interfaces/IApplicationDbContext.cs
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces // Veya WEBPROJE.Core.Application.Interfaces (seçtiğiniz namespace'e göre)
{
    public interface IApplicationDbContext
    {
        DbSet<Category> Categories { get; }
        DbSet<Product> Products { get; }
        DbSet<ProductImage> ProductImages { get; }
        DbSet<ProductTag> ProductTags { get; }
        DbSet<Tag> Tags { get; }
        DbSet<Content> Contents { get; }
        DbSet<ContentTag> ContentTags { get; }
        DbSet<ContactMessage> ContactMessages { get; }
        DbSet<CompanyInfo> CompanyInfos { get; }
        DbSet<SeoSetting> SeoSettings { get; }
        DbSet<HomePageSection> HomePageSections { get; }
        DbSet<Page> Pages { get; }
        DbSet<PageSetting> PageSettings { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

}
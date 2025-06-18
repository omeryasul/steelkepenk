using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Persistence;
using System.Reflection;

namespace Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // OnConfiguring metodunu tamamen kaldırdık

        // Domain DbSets
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<ProductTag> ProductTags { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Content> Contents { get; set; } = null!;
        public DbSet<ContentTag> ContentTags { get; set; } = null!;
        public DbSet<ContactMessage> ContactMessages { get; set; } = null!;
        public DbSet<CompanyInfo> CompanyInfos { get; set; } = null!;
        public DbSet<SeoSetting> SeoSettings { get; set; } = null!;
        public DbSet<HomePageSection> HomePageSections { get; set; } = null!;
        public DbSet<Page> Pages { get; set; } = null!;
        public DbSet<PageSetting> PageSettings { get; set; } = null!;

        // Domain User mapping (Identity'den ayrı)
        public DbSet<User> DomainUsers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Assembly configurations - HasData sorunu için geçici kapatıldı
            // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // User mapping configuration
            ConfigureUserMapping(modelBuilder);
        }

        private static void ConfigureUserMapping(ModelBuilder modelBuilder)
        {
            // Domain User entity'yi Identity Users'tan ayrı tut
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("DomainUsers");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.UserName).HasMaxLength(256);
                entity.Property(x => x.Email).HasMaxLength(256);
                entity.Property(x => x.FirstName).HasMaxLength(100);
                entity.Property(x => x.LastName).HasMaxLength(100);

                entity.HasIndex(x => x.Email).IsUnique();
                entity.HasIndex(x => x.UserName).IsUnique();
            });
        }

        private static void ConfigurePageSettings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PageSetting>(entity =>
            {
                entity.ToTable("PageSettings");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Key)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Value)
                    .HasMaxLength(4000);

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.Property(x => x.Group)
                    .HasMaxLength(50)
                    .HasDefaultValue("General");

                entity.HasIndex(x => x.Key).IsUnique();
                entity.HasIndex(x => new { x.Group, x.Key });

                // HasData kaldırıldı - Seed data'yı ayrı servis ile yapacağız
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in base.ChangeTracker.Entries<BaseAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
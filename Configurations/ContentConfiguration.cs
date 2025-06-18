// ContentConfiguration.cs - Persistence/Configurations
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Summary)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Body)  // Content yerine Body
                .IsRequired();

            builder.Property(x => x.FeaturedImage)
                .HasMaxLength(500);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.MetaTitle)
                .HasMaxLength(60);

            builder.Property(x => x.MetaDescription)
                .HasMaxLength(160);

            builder.Property(x => x.MetaKeywords)
                .HasMaxLength(500);

            builder.Property(x => x.OgTitle)
                .HasMaxLength(60);

            builder.Property(x => x.OgDescription)
                .HasMaxLength(160);

            builder.Property(x => x.OgImage)
                .HasMaxLength(500);

            builder.Property(x => x.CanonicalUrl)
                .HasMaxLength(500);

            // Computed property'yi ignore et
            builder.Ignore(x => x.ContentBody);

            builder.HasIndex(x => x.Slug).IsUnique();
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.Type);
            builder.HasIndex(x => x.CategoryId);
            builder.HasIndex(x => x.CreatedDate);
            builder.HasIndex(x => x.IsFeatured);

            // İlişkiler
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Contents)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ContentTags)
                .WithOne(x => x.Content)
                .HasForeignKey(x => x.ContentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
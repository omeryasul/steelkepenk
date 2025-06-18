using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.ShortDescription)
                .HasMaxLength(500);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.DisplayPrice)
                .HasPrecision(18, 2);

            builder.Property(x => x.PriceNote)
                .HasMaxLength(200);

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

            builder.HasIndex(x => x.Slug).IsUnique();
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.CategoryId);
            builder.HasIndex(x => x.IsFeatured);
            builder.HasIndex(x => x.CreatedDate);

            // İlişkiler
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ProductImages)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ProductTags)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

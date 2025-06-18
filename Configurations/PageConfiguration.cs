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
    public class PageConfiguration : IEntityTypeConfiguration<Page>
    {
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Summary)
                .HasMaxLength(500);

            builder.Property(x => x.Content)
                .IsRequired();

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

            builder.Property(x => x.HeroTitle)
                .HasMaxLength(200);

            builder.Property(x => x.HeroSubtitle)
                .HasMaxLength(300);

            builder.Property(x => x.HeroImage)
                .HasMaxLength(500);

            builder.Property(x => x.HeroButtonText)
                .HasMaxLength(50);

            builder.Property(x => x.HeroButtonLink)
                .HasMaxLength(500);

            builder.HasIndex(x => x.Slug).IsUnique();
            builder.HasIndex(x => x.Type);
            builder.HasIndex(x => x.Status);
        }
    }
}

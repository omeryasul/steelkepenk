using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class HomePageSectionConfiguration : IEntityTypeConfiguration<HomePageSection>
    {
        public void Configure(EntityTypeBuilder<HomePageSection> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SectionName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Subtitle)
                .HasMaxLength(300);

            builder.Property(x => x.Content)
                .HasMaxLength(2000);

            builder.Property(x => x.Image)
                .HasMaxLength(500);

            builder.Property(x => x.ButtonText)
                .HasMaxLength(50);

            builder.Property(x => x.ButtonLink)
                .HasMaxLength(500);

            builder.Property(x => x.AdditionalData)
                .HasMaxLength(4000); // JSON field için

            builder.HasIndex(x => x.SectionName);
            builder.HasIndex(x => x.SortOrder);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
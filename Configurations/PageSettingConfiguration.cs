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
    public class PageSettingConfiguration : IEntityTypeConfiguration<PageSetting>
    {
        public void Configure(EntityTypeBuilder<PageSetting> builder)
        {
            builder.ToTable("PageSettings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Value)
                .HasMaxLength(4000);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.Group)
                .HasMaxLength(50)
                .HasDefaultValue("General");

            builder.HasIndex(x => x.Key)
                .IsUnique();

            builder.HasIndex(x => new { x.Group, x.Key });

            // Seed data
            builder.HasData(
                new PageSetting
                {
                    Id = 1,
                    Key = "site_name",
                    Value = "Otomatik Kepenek Sistemleri",
                    Description = "Site başlığı",
                    Group = "SEO",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new PageSetting
                {
                    Id = 2,
                    Key = "site_description",
                    Value = "İstanbul'da otomatik kepenek sistemleri, tamiri ve bakım hizmetleri. 7/24 hızlı servis.",
                    Description = "Site açıklaması",
                    Group = "SEO",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new PageSetting
                {
                    Id = 3,
                    Key = "hero_title",
                    Value = "HIZLI SERVİS<br><span class=\"text-gradient\">KESİNTİSİZ DESTEK</span>",
                    Description = "Ana sayfa hero başlık",
                    Group = "HomePage",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new PageSetting
                {
                    Id = 4,
                    Key = "hero_phone",
                    Value = "+905336619312",
                    Description = "Hero bölümü telefon",
                    Group = "Contact",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new PageSetting
                {
                    Id = 5,
                    Key = "company_title",
                    Value = "Steel Otomatik Kapı Sistemleri - KEPENEK TAMİRİ",
                    Description = "Şirket başlığı",
                    Group = "Company",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }
    }
}

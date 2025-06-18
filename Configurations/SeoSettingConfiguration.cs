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
    public class SeoSettingConfiguration : IEntityTypeConfiguration<SeoSetting>
    {
        public void Configure(EntityTypeBuilder<SeoSetting> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SiteName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.SiteDescription)
                .IsRequired()
                .HasMaxLength(160);

            builder.Property(x => x.SiteKeywords)
                .HasMaxLength(500);

            builder.Property(x => x.SiteUrl)
                .HasMaxLength(200);

            builder.Property(x => x.GoogleAnalyticsId)
                .HasMaxLength(50);

            builder.Property(x => x.GoogleTagManagerId)
                .HasMaxLength(50);

            builder.Property(x => x.FacebookPixelId)
                .HasMaxLength(50);

            builder.Property(x => x.GoogleSearchConsoleCode)
                .HasMaxLength(100);

            builder.Property(x => x.RobotsText)
                .HasMaxLength(2000);

            builder.Property(x => x.DefaultOgImage)
                .HasMaxLength(500);
        }
    }
}

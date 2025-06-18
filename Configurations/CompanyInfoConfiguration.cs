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
    public class CompanyInfoConfiguration : IEntityTypeConfiguration<CompanyInfo>
    {
        public void Configure(EntityTypeBuilder<CompanyInfo> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Slogan)
                .HasMaxLength(200);

            builder.Property(x => x.Logo)
                .HasMaxLength(500);

            builder.Property(x => x.FaviconUrl)
                .HasMaxLength(500);

            builder.Property(x => x.Address)
                .HasMaxLength(500);

            builder.Property(x => x.Phone)
                .HasMaxLength(20);

            builder.Property(x => x.Mobile)
                .HasMaxLength(20);

            builder.Property(x => x.Email)
                .HasMaxLength(100);

            builder.Property(x => x.Website)
                .HasMaxLength(200);

            builder.Property(x => x.FacebookUrl)
                .HasMaxLength(200);

            builder.Property(x => x.TwitterUrl)
                .HasMaxLength(200);

            builder.Property(x => x.InstagramUrl)
                .HasMaxLength(200);

            builder.Property(x => x.LinkedInUrl)
                .HasMaxLength(200);

            builder.Property(x => x.YoutubeUrl)
                .HasMaxLength(200);

            builder.Property(x => x.WorkingHours)
                .HasMaxLength(100);

            builder.Property(x => x.WorkingDays)
                .HasMaxLength(100);

            builder.Property(x => x.GoogleMapsUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.Latitude)
                .HasPrecision(10, 8);

            builder.Property(x => x.Longitude)
                .HasPrecision(11, 8);
        }
    }
}

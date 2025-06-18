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
    public class ContentTagConfiguration : IEntityTypeConfiguration<ContentTag>
    {
        public void Configure(EntityTypeBuilder<ContentTag> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ContentId, x.TagId }).IsUnique();

            // İlişkiler
            builder.HasOne(x => x.Content)
                .WithMany(x => x.ContentTags)
                .HasForeignKey(x => x.ContentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Tag)
                .WithMany(x => x.ContentTags)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

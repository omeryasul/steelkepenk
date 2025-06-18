// ContactMessageConfiguration.cs - Persistence/Configurations
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .HasMaxLength(20);

            builder.Property(x => x.Company)
                .HasMaxLength(100);

            builder.Property(x => x.Subject)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.AdminReply)
                .HasMaxLength(2000);

            builder.Property(x => x.RepliedBy)
                .HasMaxLength(100);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(45);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);

            // Computed property'yi ignore et
            builder.Ignore(x => x.FullName);

            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.Email);
            builder.HasIndex(x => x.CreatedDate);
        }
    }
}
// UserConfiguration.cs - Persistence/Configurations
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
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

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Phone)
                .HasMaxLength(20);

            builder.Property(x => x.Avatar)
                .HasMaxLength(500);

            // Computed property'yi ignore et
            builder.Ignore(x => x.FullName);

            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.IsActive);
        }
    }
}
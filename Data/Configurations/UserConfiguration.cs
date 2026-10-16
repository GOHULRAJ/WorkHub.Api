using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkHub.Api.Domain.Entities;

namespace WorkHub.Api.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder) 
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}


    //HasKey() defines the primary key.
    //IsRequired() makes a property mandatory in the database.
    //HasMaxLength() defines the maximum column length.
    //HasIndex().IsUnique() prevents duplicate email addresses, including concurrent insert attempts.
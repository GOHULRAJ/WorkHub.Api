using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkHub.Api.Domain.Entities;

namespace WorkHub.Api.Data.Configurations
{
    public class RefreshTokenConfiguration:IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            //builder.ToTable("User_refresh_token");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasIndex(rt => rt.TokenHash)
                .IsUnique();

            builder.Property(rt => rt.ReplacedByTokenHash)
                .HasMaxLength(64);

            builder.Property(rt => rt.CreatedAtUtc)
                .IsRequired();

            builder.Property(rt => rt.ExpiresAtUtc)
                .IsRequired();

            builder.Property(rt => rt.UserId)                
                .IsRequired();

            builder.HasIndex(rt => rt.TokenFamilyId);

            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}

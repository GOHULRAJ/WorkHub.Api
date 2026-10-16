using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkHub.Api.Domain.Entities;

namespace WorkHub.Api.Data.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project> 
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(p => p.CreatedAtUtc)
                .IsRequired();

            builder.HasOne(p => p.CreatedByUser)
                .WithMany(u => u.CreatedProjects)
                .HasForeignKey(p => p.CreatedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.CreatedByUserId);

        }
    }
}


//HasOne() — a project has one creator.
//WithMany() — a user can create many projects.
//HasForeignKey() — CreatedByUserId stores the user's ID.
//IsRequired() — every project must have a creator.
//Restrict — the creator cannot be deleted while projects reference that user.
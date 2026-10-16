using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkHub.Api.Domain.Entities;

namespace WorkHub.Api.Data.Configurations
{
    public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {
            builder.ToTable("ProjectMembers");

            builder.HasKey(pm => new
            {
                pm.UserId,
                pm.ProjectId
            });

            builder.Property(pm => pm.JoinedAtUtc)
                .IsRequired();

            builder.HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMemberships)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pm => pm.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pm => pm.ProjectId);
        }
    }
}


//HasOne() — a project has one creator.
//WithMany() — a user can create many projects.
//HasForeignKey() — CreatedByUserId stores the user's ID.
//IsRequired() — every project must have a creator.
//Restrict — the creator cannot be deleted while projects reference that user.

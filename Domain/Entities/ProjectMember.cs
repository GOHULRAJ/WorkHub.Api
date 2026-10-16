namespace WorkHub.Api.Domain.Entities
{
    public class ProjectMember
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;

        //Navigation Properties
        public User User { get; set; } = null!;
        public Project Project { get; set; } = null!;
    }
}

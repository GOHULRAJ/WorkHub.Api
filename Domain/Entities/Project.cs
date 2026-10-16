namespace WorkHub.Api.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        
        //Foreign Key
        public int CreatedByUserId { get; set; }

        // Navigation property to the creator
        public User CreatedByUser { get; set; } = null!;

        // Navigation properties
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    }
}

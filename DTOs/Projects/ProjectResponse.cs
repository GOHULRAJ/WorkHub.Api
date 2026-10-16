namespace WorkHub.Api.DTOs.Projects
{
    public class ProjectResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public int CreatedByUserId { get; set; }
    }
}

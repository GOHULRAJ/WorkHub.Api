namespace WorkHub.Api.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";

        //Required Project Relationship
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        //Optional Assigned-user Relationship
        public int? AssignedUserId { get; set; } 
        public User? AssignedUser { get; set; }
    }
}

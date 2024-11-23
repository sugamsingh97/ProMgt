namespace ProMgt.Client.Models.Assignments
{
    public class ProjectAssignedDisplayModel
    {
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; } = string.Empty;
        public string? AsigneeId { get; set; }
        public string? UserId { get; set; }

    }
}

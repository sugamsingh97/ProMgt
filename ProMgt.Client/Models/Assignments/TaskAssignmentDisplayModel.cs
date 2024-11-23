namespace ProMgt.Client.Models.Assignments
{
    public class TaskAssignmentDisplayModel
    {
        public int? TaskId { get; set; }
        public string? TaskName { get; set; } = string.Empty;
        public string? AsigneeId { get; set; }
        public string? UserId { get; set; }
    }
}

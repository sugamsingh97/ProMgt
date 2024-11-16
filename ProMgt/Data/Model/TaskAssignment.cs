using System.ComponentModel.DataAnnotations.Schema;

namespace ProMgt.Data.Model
{
    public class TaskAssignment
    {
        public int Id { get; set; } // Unique identifier for the assignment
        public string? UserId { get; set; } // ID of the assignee
        public string? AssigneeId { get; set; } // ID of the assignee

        [ForeignKey("ProjectTask")]
        public int? TaskId { get; set; }
        public virtual ProjectTask ProjectTask { get; set; }
    }
}

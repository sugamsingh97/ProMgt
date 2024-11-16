using System.ComponentModel.DataAnnotations.Schema;

namespace ProMgt.Data.Model
{
    public class ProjectAssignment
    {
        public int Id { get; set; } // Unique identifier for the assignment
        public string? UserId { get; set; } // ID of the assignee
        public string? AssigneeId { get; set; } // ID of the assignee

        [ForeignKey("Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }

       
    }
}

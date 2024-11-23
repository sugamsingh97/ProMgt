using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Assignments
{
    public class TaskAssignmentInputModel
    {
        [Required]
        public string? AssigneeId { get; set; }   // Id of the user to be assigned to a project

        [Required]
        public int TaskId { get; set; }
    }
}

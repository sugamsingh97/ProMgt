using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProMgt.Client.Models.Assignments
{
    public class ProjectAssignedInputModel
    {
        [Required]
        public string? AssigneeId { get; set; }   // Id of the user to be assigned to a project

        [Required]
        public int ProjectId { get; set; }
    }
}

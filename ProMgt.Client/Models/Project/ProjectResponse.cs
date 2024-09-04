using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Project
{
    public class ProjectResponse
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime DateOfCreation { get; set; }
        public DateTime? DeadLine { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        // public virtual ICollection<ProjectTask>? Tasks { get; set; }
    }
}

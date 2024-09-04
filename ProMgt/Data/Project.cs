using System.ComponentModel.DataAnnotations;

namespace ProMgt.Data
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = String.Empty;

        public string? Description { get; set; }

        public DateTime DateOfCreation { get; set; }
        public DateTime? DeadLine { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public virtual ICollection<ProjectTask>? Tasks { get; set; }

    }
}

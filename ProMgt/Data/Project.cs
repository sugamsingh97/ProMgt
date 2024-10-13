using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("ProjectStatus")]
        public int? ProjectStatusId { get; set; }
        public virtual ProjectStatus? ProjectStatus { get; set; }

        public virtual ICollection<ProjectTask>? Tasks { get; set; }
        public virtual ICollection<Priority>? Priorities { get; set; }
        public virtual ICollection<TaskStatus>? TaskStatuses { get; set; }
    }
}

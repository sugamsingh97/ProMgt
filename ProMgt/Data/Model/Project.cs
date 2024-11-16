using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProMgt.Data.Model
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProjectSummery { get; set; }
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
        public virtual ICollection<Section>? Sections { get; set; }       
        public virtual ICollection<ProjectAssignment>? ProjectAssignments { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using SQLitePCL;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProMgt.Data.Model
{
    public class ProjectTask
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? ProjectSummery { get; set; }
        public DateTime DateOfCreation { get; set; }
        public DateTime? DeadLine { get; set; }
        [Required]
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        [ForeignKey("Priority")]
        public int? PriorityId { get; set; }
        public virtual Priority? Priority { get; set; }

        [ForeignKey("TaskStatus")]
        public int? TaskStatusId { get; set; }
        public virtual TaskStatus? TaskStatus { get; set; }

        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        [ForeignKey("Section")]
        public int? SectionId { get; set; }
        public virtual Section? Section { get; set; }

        public virtual ICollection<TaskAssignment>? TaskAssignments { get; set; }
    }

}

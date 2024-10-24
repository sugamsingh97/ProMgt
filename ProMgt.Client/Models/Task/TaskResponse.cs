using ProMgt.Client.Models.Project;
using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Task
{
    public class TaskResponse
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int ProjectId { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? TaskSummery { get; set; }

        public DateTime DateOfCreation { get; set; }

        public DateTime? DeadLine { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
        public int? PriorityId { get; set; }
        public int? TaskStatusId { get; set; }
        public int SectionId { get; set; }
        public string? PriorityName { get; set; }
        public string? TaskStatusName { get; set; }

        public string? PriorityHexcode { get; set; }
        public string? TaskStatusHexcode { get; set; }

        public virtual ProjectResponse Project { get; set; } = new();
    }
}

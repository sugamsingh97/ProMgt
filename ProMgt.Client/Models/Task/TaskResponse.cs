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

        public DateTime DateOfCreation { get; set; }

        public DateTime? DeadLine { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public virtual ProjectResponse Project { get; set; } = new();
    }
}

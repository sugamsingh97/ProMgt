using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Fields.TaskStatus
{
    public class TaskStatusInputModel
    {
        [Required]
        public string? Name { get; set; } = string.Empty;

        [Required]
        public int ColorId { get; set; }

        public int ProjectId { get; set; }
    }
}

using ProMgt.Client.Models.Project;
using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Task
{
    public class TaskDisplay
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public DateTime? DateOfCreation { get; set; }
        public DateTime? DeadLine { get; set; }      
        public bool IsCompleted { get; set; }
        public string? Description { get; set; } = string.Empty;

        public int? PriorityId { get; set; }
        public int? TaskStatusId { get; set; }
    }
}

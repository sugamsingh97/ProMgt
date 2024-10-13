using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Fields.TaskStatus
{
    public class TaskStatusResponse
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; } = string.Empty;

        [Required]
        public int ColorId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public string? HexColor { get; set; }

    }
}

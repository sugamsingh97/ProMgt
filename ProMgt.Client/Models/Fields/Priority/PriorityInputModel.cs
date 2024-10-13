using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Fields.Priority
{
    public class PriorityInputModel
    {
        [Required]
        public string? Name { get; set; } = string.Empty;

        [Required]
        public int ColorId { get; set; }

        public int projectId { get; set; }
    }
}

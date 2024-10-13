using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Fields.Priority
{
    public class PriorityResponse
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; } = string.Empty;

        [Required]
        public int ColorId { get; set; }

        [Required]
        public int projectId { get; set; }

        public string? HexColor { get; set; }

    }
}

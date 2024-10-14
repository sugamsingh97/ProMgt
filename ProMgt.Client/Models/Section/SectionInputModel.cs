using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Section
{
    public class SectionInputModel
    {
        [Required]
        public string? Name { get; set; } = string.Empty;
        public int ProjectId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Project
{
    public class ProjectStatusResponse
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string? HexCode { get; set; }

    }
}

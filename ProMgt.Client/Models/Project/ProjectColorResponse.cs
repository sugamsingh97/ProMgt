using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Project
{
    public class ProjectColorResponse
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? HexCode { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ProMgt.Data.Model
{
    public class ProjectStatus
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string? HexCode { get; set; }

        public virtual ICollection<Project>? Projects { get; set; }
    }

}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProMgt.Data.Model
{
    public class Section
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public virtual ICollection<ProjectTask>? Tasks { get; set; }
    }
}

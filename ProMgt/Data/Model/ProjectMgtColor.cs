using System.ComponentModel.DataAnnotations;

namespace ProMgt.Data.Model
{
    public class ProjectMgtColor
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? HexCode { get; set; }

        public virtual ICollection<Priority>? Priorities { get; set; }
        public virtual ICollection<TaskStatus>? TaskStatuses { get; set; }
    }

}

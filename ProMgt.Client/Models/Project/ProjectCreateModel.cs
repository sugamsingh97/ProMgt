using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Project
{
    public class ProjectCreateModel
    {

        [Display(Name = "Project Name")]
        [Required(ErrorMessage = "Project Name should not be empty")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Deadline")]
        public DateTime? DeadLine { get; set; } = DateTime.Now;

        [MaxLength(200)]
        public string? Description { get; set; }

    }
}

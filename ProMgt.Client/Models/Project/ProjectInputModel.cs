using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ProMgt.Client.Models.Project
{
    public class ProjectInputModel
    {
        public int? Id { get; set; }

        [Display(Name = "Project Name")]
        [Required(ErrorMessage = "Project Name should not be empty")]        
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Deadline")]
        public DateTime? DeadLine { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

    }   
}

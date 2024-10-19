using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Task
{
    public class TaskInputModel
    {
        [Display(Name = "Task Name")]
        [Required(ErrorMessage = "This field is Required")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "DeadLine")]
        public DateTime? DeadLine { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public string? Description { get; set; } = string.Empty;

        public int? SectionId { get; set; }
    }
}

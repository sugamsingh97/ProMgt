using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using SQLitePCL;
namespace ProMgt.Data
{
    public class ProjectTask
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int ProjectId { get; set; }
       
        public string Description { get; set; } = string.Empty;

        public DateTime DateOfCreation { get; set; }

        public DateTime? DeadLine { get; set; }

        [Required]
        public string CreatedBy { get; set; } = String.Empty;

        public bool IsCompleted { get; set; }
        
        public virtual Project Project { get; set; } = new();

    }
}

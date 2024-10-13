using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Project
{
    public class ProjectDisplay
    {

        public int Id { get; set; }       
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfCreation { get; set; }
        public DateTime? DeadLine { get; set; }
        public bool IsCompleted { get; set; }
        public int? ProjectStatusId { get; set; }

    }
}

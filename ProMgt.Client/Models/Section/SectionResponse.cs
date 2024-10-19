using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.Section
{
    public class SectionResponse
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;        

        [Required]
        public int ProjectId { get; set; }

    }
}

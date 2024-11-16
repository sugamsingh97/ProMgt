using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProMgt.Client.Models.User
{
    public class ContactResponse
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }       
        public string? UserId { get; set; }

        [Required]
        public string ContactUserId { get; set; }     
    }
}

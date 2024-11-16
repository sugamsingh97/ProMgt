using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.User
{
    public class ContactDisplay
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserId { get; set; }       
        public string? ContactUserId { get; set; }
        public string? ContactFirstName { get; set; }
        public string? ContactLastName { get; set; }
        public string? ContactEmail { get; set; }

    }
}

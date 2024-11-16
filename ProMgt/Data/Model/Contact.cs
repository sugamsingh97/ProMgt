using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProMgt.Data.Model
{
    public class Contact
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("ContactUser")]
        [Required]
        public string? ContactUserId { get; set; }
        public virtual ApplicationUser ContactUser { get; set; }

    }

}

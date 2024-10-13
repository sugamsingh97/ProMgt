using ProMgt.Client.Models.AuthModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProMgt.Data
{
    public class Contact
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        public int ProjectOwner { get; set; }

        public bool IsUser { get; set; }

        public Guid? UserId { get; set; }

        [NotMapped]
        public List<RoleItem>? Roles { get; set; }
    }
}

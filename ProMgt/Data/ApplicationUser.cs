using Microsoft.AspNetCore.Identity;
using ProMgt.Client.Models.AuthModels;
using System.ComponentModel.DataAnnotations.Schema;
using ProMgt.Data.Model;

namespace ProMgt.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Title { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool DarkMode { get; set; }
    public bool EmailNotifPref { get; set; }
    public Byte[]? ProfilePicture { get; set; }

    [NotMapped]
    public List<RoleItem> Roles { get; set; } = new();

    // Navigation properties for contacts
    public virtual ICollection<Contact>? Contacts { get; set; }
    public virtual ICollection<Contact>? ContactedBy { get; set; }
}



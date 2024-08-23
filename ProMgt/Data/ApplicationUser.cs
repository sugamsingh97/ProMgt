using Microsoft.AspNetCore.Identity;
using ProMgt.Client.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProMgt.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    /*
     * FirstName
       * LastName
        *Title
       Email
    Password
       * Date of Birthday
       * Email notif pref?(grou)
    *
       * Dark Mode Choice
       * Profile Picture
     * */

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? Title { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool DarkMode { get; set; }

    public bool EmailNotifPref { get; set; }

    public Byte[]? ProfilePicture { get; set; }

    [NotMapped]
    public List<RoleItem> Roles { get; set; } = new();
}



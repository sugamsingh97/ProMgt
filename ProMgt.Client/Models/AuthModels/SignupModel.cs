using ProMgt.Client.Infrastructure.Validators;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProMgt.Client.Models.AuthModels
{
    public class SignupModel
    {
        [DisplayName("Title")]
        public string? Title { get; set; } = string.Empty;

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First name is required!")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "First name must contain only alphabetical characters")]
        public string FirstName { get; set; } = string.Empty;

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last name is required!")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Last name must contain only alphabetical characters")]
        public string LastName { get; set; } = string.Empty;

        [DisplayName("Date of birth")]
        [DateNotInFuture]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required!")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and the confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [DisplayName("Profile Picture")]
        public byte[]? ProfilePicture { get; set; }

        [DisplayName("Receive Email notifications")]
        public bool EmailNotifPref { get; set; }

        [DisplayName("Dark mode")]
        public bool DarkModePref { get; set; }
    }
}

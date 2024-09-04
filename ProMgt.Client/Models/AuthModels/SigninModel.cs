using ProMgt.Client.Infrastructure.Validators;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace ProMgt.Client.Models.AuthModels
{
    public class SigninModel
    {
        [DisplayName("Email")]
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required!")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;

    }
}

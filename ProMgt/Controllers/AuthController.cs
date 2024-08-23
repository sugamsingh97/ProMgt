using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using MudBlazor;
using ProMgt.Client.Models;
using ProMgt.Data;
using System.Text;
using System.Text.Encodings.Web;

namespace ProMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IEmailSender<ApplicationUser> _emailSender;
        private readonly IUserStore<ApplicationUser> _userStore;
        private IEnumerable<IdentityError>? identityErrors;
        private readonly ILogger<AuthController> _logger;
        public AuthController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AuthenticationStateProvider authStateProvider,
            IUserStore<ApplicationUser> userStore,
            IEmailSender<ApplicationUser> emailSender,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authStateProvider = authStateProvider;
            _userStore = userStore;
            _emailSender = emailSender;
            _logger = logger;
        }

        // we are writing the endpoint. it can be anything, we wrote "signin"
        [HttpPost("signin")]
        public async Task<ActionResult<SigninModel>> SignIn(SigninModel model)
        {
            // Server check of validity of the SigninModel.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Sign in successful" });
            }
            if (result.IsLockedOut)
            {
                return BadRequest("User account is Locked out");
            }
            return BadRequest("Invalid login attempt.");
        }

        [HttpPost("sendconflink")]
        public async Task<IActionResult> SendConfirmationLink(string email, string callbackUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (callbackUrl == null)
            {
                return BadRequest("Failed to generate confirmation URL.");
            }
            await _emailSender.SendConfirmationLinkAsync(user, email, callbackUrl);
            return Ok("Confirmation link sent. Please check your email.");
        }

        [HttpPost("signup")]
        public async Task<ActionResult<SignupModel>> SignUp(SignupModel model)
        {
            // Server check of validity of the SignUpModel.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = CreateUser();

            user.UserName = model.Email;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Title = model.Title;
            user.DateOfBirth = model.DateOfBirth;
            user.DarkMode = model.DarkModePref;
            user.ProfilePicture = model.ProfilePicture;
            

            await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
            var emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(identityErrors);
            }
            _logger.LogInformation("User created a new account with a password.");
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

    //        var callbackUrl = Url.Action(
    //            "ConfirmEmail", "Auth",
    //            new { userId = userId, code = code },
    //            protocol: HttpContext.Request.Scheme);

    //        if (callbackUrl == null)
    //        {
    //            return BadRequest("Failed to generate confirmation URL.");
    //        }
    //        await _emailSender.SendConfirmationLinkAsync(user, "Confirm your email",
    //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Ok(new
            {
                userId,
                code,
                requireConfirmedAccount = _userManager.Options.SignIn.RequireConfirmedAccount
            });

        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }
    }
}

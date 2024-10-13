using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using ProMgt.Data;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ProMgt.Infrastructure.Email
{
    public class EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                       ILogger<EmailSender> logger) : IEmailSender<ApplicationUser>
    {
        private readonly ILogger _logger = logger;

        public AuthMessageSenderOptions Options { get; } = optionsAccessor.Value;

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(Options.SendGridKey))
            {
                throw new Exception("Null SendGridKey");
            }
            await Execute(Options.SendGridKey, subject, message, toEmail);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("info@promgt.com", "Password Recovery"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
#pragma warning disable CA2254 // Template should be a static expression
            _logger.LogInformation(response.IsSuccessStatusCode
                                   ? $"Email to {toEmail} queued successfully!"
                                   : $"Failure Email to {toEmail}");
#pragma warning restore CA2254 // Template should be a static expression
        }

        public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            //var apiKey = Options.SendGridKey;
            //var client = new SendGridClient(apiKey);
            //var from = new EmailAddress("anupjitamang@gmail.com", "Anup Tamang");
            //var subject = "Confirm your account";
            //var to = new EmailAddress(email, user.UserName);
            //var plainTextContent = "and easy to do anywhere, even with C#";
            //var htmlContent = "<strong>Just one more step click this link <a href='" + confirmationLink + "'>Confirm my account</a> </strong>";
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //var response = await client.SendEmailAsync(msg);
            if (user is not null && user.UserName is not null && user.UserName != string.Empty)
            {
                var apiKey = Options.SendGridKey;
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("anupjitamang@gmail.com", "Anup Tamang");
                EmailAddress to = new(email, user.UserName);

                var msg = new SendGridMessage()
                {
                    From = from,
                    Subject = "Confirm your account",
                    TemplateId = "d-1a7d24ccff7e4874991c1a818cb1e775", // Replace with your actual template ID
                    Personalizations =
                    [
                        new Personalization
                    {
                        Tos = [new(email, user.UserName)],
                        TemplateData = new Dictionary<string, string>
                        {
                            { "first_name", user.UserName },
                            { "Url", confirmationLink }
                        }
                    }
                    ]
                };

                var response = await client.SendEmailAsync(msg);
                if (!response.IsSuccessStatusCode)
                {

                }
            }
        }

        public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            string? apiKey = Options.SendGridKey;
            SendGridClient client = new SendGridClient(apiKey);
            EmailAddress from = new EmailAddress("rudeboyhere@gmail.com", "Sam Singh");
            EmailAddress to = new(email, user.UserName);

            var msg = new SendGridMessage()
            {
                From = from,
                Subject = "Password reset",
                TemplateId = "d-a6e27e8306fe4832b816e86cf57261b4", // Replace with your actual template ID
                Personalizations =
                [
                    new Personalization
                    {
                        Tos = [new(email, user.UserName)],
                        TemplateData = new Dictionary<string, string>
                        {
                            { "Url", resetLink }
                        }
                    }
                ]
            };
            var response = await client.SendEmailAsync(msg);
            if (!response.IsSuccessStatusCode)
            {

            }
            //var apiKey = Options.SendGridKey;
            //var client = new SendGridClient(apiKey);
            //var from = new EmailAddress("anupjitamang@gmail.com", "Anup Tamang");
            //var subject = "Confirm your account";
            //var to = new EmailAddress(email, user.UserName);
            //var plainTextContent = "Just one more step to reset your password click this link your browser" + resetLink;
            //var htmlContent = "<strong>Just one more step to reset your password click this link<br /> <br /><a href='" + resetLink + "'>Reset password</a> </strong>";
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //var response = await client.SendEmailAsync(msg);

        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }
    }
}
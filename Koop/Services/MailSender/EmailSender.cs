using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Koop.Services
{
    public class EmailSender : IMyEmailSender
    {
        private string PasswordResetLayout = "Services/MailSender/Layouts/PasswordReset.html";
        private IEmailSender _emailSender;
        private ILogger<EmailSender> _logger;

        public EmailSender(IEmailSender emailSender, ILogger<EmailSender> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task SendPasswordResetToken(string email, string link)
        {
            try
            {
                var passwordResetLayout = Path.Combine(Directory.GetCurrentDirectory(), PasswordResetLayout);
                using (StreamReader sr = new StreamReader(passwordResetLayout))
                {
                    string layout = sr.ReadToEnd().Replace("#link#", link);

                    await _emailSender.SendEmailAsync(email, "Koop - resetowanie hasła", layout);
                    
                    _logger.LogInformation("Email with reset link was successfully sent to {Email}", email);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(37, "{Msg}", e.Message);
            }
        }
    }
}
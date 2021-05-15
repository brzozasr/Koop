using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Threading.Tasks;
using Koop.Models.MailSenderService;
using Microsoft.Extensions.Options;

namespace Koop.Services
{
    public class EmailSenderService : IEmailSender
    {
        private MailOptions _mailOptions;
        private SmtpClient _smtpClient;
        
        public EmailSenderService(IOptions<MailOptions> mailOptions)
        {
            _mailOptions = mailOptions.Value;
            _smtpClient = new SmtpClient(_mailOptions.Host, _mailOptions.Port);
            _smtpClient.Credentials = new NetworkCredential(_mailOptions.UserName, _mailOptions.Password);
            _smtpClient.EnableSsl = true;
        }
        
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var msg = new MailMessage(_mailOptions.UserName, email, subject, htmlMessage);
            msg.IsBodyHtml = true;
            
            _smtpClient.Send(msg);
            
            return Task.CompletedTask;
        }
    }
}
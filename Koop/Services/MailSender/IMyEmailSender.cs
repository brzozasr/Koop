using System.Threading.Tasks;

namespace Koop.Services.MailSender
{
    public interface IMyEmailSender
    {
        public Task SendPasswordResetToken(string email, string link);
        public Task SendOrderConfirmation(string email, string tbody, string tfoot);
    }
}
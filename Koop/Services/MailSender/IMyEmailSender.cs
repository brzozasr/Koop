using System.Threading.Tasks;

namespace Koop.Services
{
    public interface IMyEmailSender
    {
        public Task SendPasswordResetToken(string email, string link);
    }
}
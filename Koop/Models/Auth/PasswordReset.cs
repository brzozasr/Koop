using System;

namespace Koop.Models.Auth
{
    public class PasswordReset
    {
        public string Email { get; set; }
        public string HostName { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
        public string UserId { get; set; }
    }
}
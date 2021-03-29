using System.ComponentModel.DataAnnotations;

namespace Koop.Models.Auth
{
    public class UserSignUp
    {
        [EmailAddress]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace Koop.Models.Auth
{
    public class UserSignUp
    {
        [EmailAddress]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        private string _info;
        public string Info
        {
            get => _info;
            set => _info = value == String.Empty ? null : value;
        }

        public double? Debt { get; set; }

        private Guid? _fundId;
        public Guid? FundId
        {
            get => _fundId;
            set => _fundId = value == Guid.Empty ? null : value;
        }

        private Guid? _basketId;
        public Guid? BasketId
        {
            get => _basketId;
            set => _basketId = value == Guid.Empty ? null : value;
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value == String.Empty ? null : value;
        }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
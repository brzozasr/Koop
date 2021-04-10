using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;

namespace Koop.Models.Auth
{
    public class UserEdit
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Info { get; set; }
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
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string OldPassword { get; set; }
        [StringLength(100, MinimumLength = 8, ErrorMessage = "The password must be at least 8 characters long.")]
        public string NewPassword { get; set; }
    }
}
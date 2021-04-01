using System;
using Microsoft.AspNetCore.Mvc;

namespace Koop.Models.Auth
{
    [BindProperties]
    public class UserAddRole
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
    }
}
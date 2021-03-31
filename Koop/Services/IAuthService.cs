using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Koop.Models;
using Koop.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Koop.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> SignUp([FromBody] UserSignUp userSignUp);
        string SignIn([FromBody] UserLogIn userLogIn);
        Task<IdentityResult> CreateRole(string roleName);
        Task<IdentityResult> AddUserToRole(Guid id, [FromBody] string roleName);
        public Task<IdentityResult> EditUser([FromBody] UserEdit userEdit);
        public User GetUser(Guid userId);
    }
}
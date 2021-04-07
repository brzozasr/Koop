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
        Task<IdentityResult> SignUp([FromBody]UserEdit newUser);
        string SignIn([FromBody] UserLogIn userLogIn);
        Task<IdentityResult> CreateRole(string roleName);
        Task<IdentityResult> AddRoleToUser(Guid id, [FromBody] string roleName);
        public Task<IdentityResult> EditUser(UserEdit userEdit, Guid userId, Guid authUserId, IEnumerable<string> authUserRoles);
        public UserEdit GetUser(Guid userId);
        public Task<IdentityResult> RemoveUser(Guid userId);
        public Task<IdentityResult> RemoveRoleFromUser(Guid userId, string roleName);
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Koop.Models;
using Koop.Models.Auth;
using Koop.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Koop.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> SignUp([FromBody]UserEdit newUser);
        public Task<RefreshToken> SignIn(UserLogIn userLogIn);
        Task<IdentityResult> CreateRole(string roleName);
        Task<IdentityResult> AddRoleToUser(Guid id, [FromBody] string roleName);
        public Task<ProblemResponse> EditUser(UserEdit userEdit, Guid userId, Guid authUserId, IEnumerable<string> authUserRoles);
        public UserEdit GetUser(Guid userId);
        public Task<IdentityResult> RemoveUser(Guid userId);
        public Task<IdentityResult> RemoveRoleFromUser(Guid userId, string roleName);
        public string GenerateJwt(User user, IList<string> roles);
        public Task<IList<string>> GetRoles(User user);
        public Task<User> GetUserRaw(Guid userId);
        public Task<RefreshToken> GetNewToken(Guid userId);
        public string GenerateRefreshToken();
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        public IEnumerable<UserEdit> GetAllUsers(Expression<Func<User, object>> orderBy, int start, int count,
            OrderDirection orderDirection = OrderDirection.Asc);

        public Task<bool> EmailDuplicationCheck(string email);
        public Task<bool> UserDuplicationCheck(string email);
    }
}
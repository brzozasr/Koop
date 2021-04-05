using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Koop.Models;
using Koop.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Koop.Services
{
    public enum AuthResponses
    {
        UserNotFound,
        EmailOrPasswordIncorrect,
        RoleNameMustBeProvided
    }
    
    public class AuthService : IAuthService
    {
        private IMapper _mapper;
        private UserManager<User> _userManager;
        private RoleManager<Role> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IMapper mapper, UserManager<User> userManager, RoleManager<Role> roleManager,
            IOptionsSnapshot<JwtSettings> jwtSettings)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }
        
        public Task<IdentityResult> SignUp([FromBody]UserSignUp userSignUp)
        {
            var user = _mapper.Map<User>(userSignUp);
            return _userManager.CreateAsync(user, userSignUp.Password);
        }

        public string SignIn(UserLogIn userLogIn)
        {
            var user = _userManager.Users
                .SingleOrDefault(p => p.NormalizedUserName == userLogIn.UserName.ToUpper() || p.NormalizedEmail == userLogIn.Email.ToUpper());
            
            if (user is null)
            {
                return null;
            }
            
            var userSignInResult = _userManager.CheckPasswordAsync(user, userLogIn.Password);

            if (userSignInResult.Result)
            {
                var roles = _userManager.GetRolesAsync(user);
                return GenerateJwt(user, roles.Result);
            }

            return null;
        }

        public UserEdit GetUser(Guid userId)
        {
            var user = _userManager.Users
                .SingleOrDefault(p => p.Id == userId);

            if (user is not null)
            {
                UserEdit userEdit = new UserEdit();

                var userOutput = _mapper.Map(user, userEdit);

                return userOutput;
            }

            return null;
        }
        
        public Task<IdentityResult> EditUser([FromBody]UserEdit userEdit)
        {
            var user = _userManager.Users
                .SingleOrDefault(p => p.Id == userEdit.Id);

            var updatedUser = _mapper.Map(userEdit, user);
            
            return _userManager.UpdateAsync(updatedUser);
        }
        
        public Task<IdentityResult> CreateRole(string roleName)
        {
            var newRole = new Role() {Name = roleName};

            return _roleManager.CreateAsync(newRole);
        }

        public Task<IdentityResult> AddUserToRole(Guid id, string roleName)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == id);

            return _userManager.AddToRoleAsync(user, roleName);
        }
        
        private string GenerateJwt(User user, IList<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            //claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.ExpirationInMinutes));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
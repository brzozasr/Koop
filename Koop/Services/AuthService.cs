using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Koop.Models;
using Koop.Models.Auth;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Microsoft.AspNetCore.Authorization;
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
        
        /*public Task<IdentityResult> SignUp([FromBody]UserSignUp userSignUp)
        {
            var user = _mapper.Map<User>(userSignUp);
            return _userManager.CreateAsync(user, userSignUp.Password);
        }*/
        
        public async Task<IdentityResult> SignUp([FromBody]UserEdit newUser)
        {
            var user = _mapper.Map<User>(newUser);
            var isEmailAlreadyPresent = await EmailDuplicationCheck(user.Email.ToUpper());
            
            if (isEmailAlreadyPresent)
            {
                return null;
            }
            
            return await _userManager.CreateAsync(user, newUser.NewPassword);
        }

        public async Task<bool> EmailDuplicationCheck(string email)
        {
            var emailsCount = await _userManager.Users.Where(p => p.NormalizedEmail.Equals(email)).CountAsync();
            return emailsCount > 0;
        }
        
        public async Task<bool> UserDuplicationCheck(string username)
        {
            var usernameCount = await _userManager.Users.Where(p => p.NormalizedUserName.Equals(username)).CountAsync();
            return usernameCount > 0;
        }

        public async Task<RefreshToken> SignIn(UserLogIn userLogIn)
        {
            /*var user = _userManager.Users
                .SingleOrDefault(p => p.NormalizedUserName == userLogIn.UserName.ToUpper() || p.NormalizedEmail == userLogIn.Email.ToUpper());*/
            //Console.WriteLine($"UserData: {userLogIn.Email} {userLogIn.Password} {userLogIn.UserName}");
            var user = _userManager.Users
                .SingleOrDefault(p => p.NormalizedEmail == userLogIn.Email.ToUpper());
            
            if (user is null)
            {
                return null;
            }
            
            var userSignInResult = await _userManager.CheckPasswordAsync(user, userLogIn.Password);
            
            if (userSignInResult)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExp = DateTime.Now.Add(TimeSpan.FromMinutes(_jwtSettings.RefreshTokenExpirationInMinutes));
                user.RefreshToken = refreshToken;
                user.RefreshTokenExp = refreshTokenExp;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return new RefreshToken()
                    {
                        Token = GenerateJwt(user, roles),
                        RefreshT = refreshToken,
                        UserId = user.Id,
                        TokenExp = _jwtSettings.ExpirationInMinutes * 60,
                        RefTokenExp = _jwtSettings.RefreshTokenExpirationInMinutes * 60
                    };
                }
            }

            return null;
        }

        public Task<IList<string>> GetRoles(User user)
        {
            return _userManager.GetRolesAsync(user);
            
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

        public Task<User> GetUserRaw(Guid userId)
        {
            return _userManager.FindByIdAsync(userId.ToString());
        }
        
        public async Task<IdentityResult> EditUser(UserEdit userEdit, Guid userId, Guid authUserId, IEnumerable<string> authUserRoles)
        {
            if (userId == authUserId || authUserRoles.Any(p => p == "Admin"))
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user is not null)
                {
                    var setEmailResult = _userManager.SetEmailAsync(user, userEdit.Email).Result;
                    if (!setEmailResult.Succeeded)
                    {
                        return setEmailResult;
                    }
                    
                    var setUserNameResult = _userManager.SetUserNameAsync(user, userEdit.UserName).Result;
                    if (!setUserNameResult.Succeeded)
                    {
                        return setUserNameResult;
                    }
                    
                    var setPhoneNumberResult = _userManager.SetPhoneNumberAsync(user, userEdit.PhoneNumber).Result;
                    if (!setPhoneNumberResult.Succeeded)
                    {
                        return setPhoneNumberResult;
                    }

                    user.BasketId = userEdit.BasketId;
                    user.FundId = userEdit.FundId;
                    user.Debt = userEdit.Debt;
                    user.Info = userEdit.Info;
                    user.FirstName = userEdit.FirstName;
                    user.LastName = userEdit.LastName;

                    if (userEdit.OldPassword is not null)
                    {
                        var changePasswordResult = await _userManager.ChangePasswordAsync(user, userEdit.OldPassword, userEdit.NewPassword);
                        if (!changePasswordResult.Succeeded)
                        {
                            return changePasswordResult;
                        }
                    }
                }
            
                return await _userManager.UpdateAsync(user);
            }

            return null;
        }
        
        public Task<IdentityResult> RemoveUser(Guid userId)
        {
            var user = _userManager.FindByIdAsync(userId.ToString());

            return _userManager.DeleteAsync(user.Result);
        }
        
        public Task<IdentityResult> CreateRole(string roleName)
        {
            var newRole = new Role() {Name = roleName};

            return _roleManager.CreateAsync(newRole);
        }

        public Task<IdentityResult> AddRoleToUser(Guid userId, string roleName)
        {
            var user = _userManager.FindByIdAsync(userId.ToString());

            return _userManager.AddToRoleAsync(user.Result, roleName);
        }
        
        public Task<IdentityResult> RemoveRoleFromUser(Guid userId, string roleName)
        {
            var user = _userManager.FindByIdAsync(userId.ToString());

            return _userManager.RemoveFromRoleAsync(user.Result, roleName);
        }
        
        public string GenerateJwt(User user, IList<string> roles)
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

        public async Task<RefreshToken> GetNewToken(Guid userId)
        {
            var user = await GetUserRaw(userId);

            if (user is null)
            {
                return null;
            }

            var roles = await GetRoles(user);
            var newToken = GenerateJwt(user, roles);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExp = DateTime.Now.Add(TimeSpan.FromMinutes(_jwtSettings.RefreshTokenExpirationInMinutes));
            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                return new RefreshToken()
                {
                    Token = newToken,
                    UserId = userId,
                    RefreshT = newRefreshToken,
                    TokenExp = _jwtSettings.ExpirationInMinutes * 60,
                    RefTokenExp = _jwtSettings.RefreshTokenExpirationInMinutes * 60
                };
            }

            return null;
        }
        
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()){
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public IEnumerable<UserEdit> GetAllUsers(Expression<Func<User, object>> orderBy, int start, int count,
            OrderDirection orderDirection = OrderDirection.Asc)
        {
            var users = _userManager.Users;
            
            var usersSorted = orderDirection == OrderDirection.Asc ? users.OrderBy(orderBy) : users.OrderByDescending(orderBy);
            var usersGrouped = usersSorted.Skip(start).Take(count);

            var usersOutput = _mapper.Map<IEnumerable<User>, IEnumerable<UserEdit>>(usersGrouped);
            
            return usersOutput;
        }
    }
}
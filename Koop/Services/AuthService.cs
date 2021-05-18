using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
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
using Microsoft.OpenApi.Writers;

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
        private IMyEmailSender _emailSender;

        public AuthService(
            IMapper mapper, 
            UserManager<User> userManager, 
            RoleManager<Role> roleManager,
            IOptionsSnapshot<JwtSettings> jwtSettings,
            IMyEmailSender emailSender)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _emailSender = emailSender;
        }
        
        /*public Task<IdentityResult> SignUp([FromBody]UserSignUp userSignUp)
        {
            var user = _mapper.Map<User>(userSignUp);
            return _userManager.CreateAsync(user, userSignUp.Password);
        }*/
        
        public async Task<ProblemResponse> SignUp([FromBody]UserEdit newUser)
        {
            ProblemResponse problemResponse = new ProblemResponse()
            {
                Detail = "Jakiś nieznany problem pojawił się w trakcie tworzenia nowego użytkownika",
                Status = 500
            };
            
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var user = _mapper.Map<User>(newUser);
                
                var isEmailAlreadyPresent = await EmailDuplicationCheck(user.Email.ToUpper());
                
                if (isEmailAlreadyPresent)
                {
                    throw new Exception("Podany adres e-mail już istnieje. Proszę podać inny");
                }
                
                var createUserResult = await _userManager.CreateAsync(user, newUser.NewPassword);

                if (!createUserResult.Succeeded)
                {
                    var code = createUserResult.Errors.FirstOrDefault().Code;
                    throw new Exception($"Problem przy tworzeniu użytkownika. Kod błędu: {code}");
                }
                
                var createdUser = await _userManager.FindByEmailAsync(newUser.Email);
                if (createdUser is null)
                {
                    throw new Exception("Pomimo utworzenia konta użytkownika, w bazie danych nie jest od obecny");
                }

                var roles = newUser.Role.ToList();
                if (!roles.Contains("Default"))
                {
                    roles.Add("Default");
                }
                var addRolesResult = await _userManager.AddToRolesAsync(createdUser, roles);
                if (!addRolesResult.Succeeded)
                {
                    throw new Exception("Błąd w trakcie dodawania roli do użytkownika");
                }

                problemResponse.Detail = "Konto użytkownika zostało utworzone";
                problemResponse.Status = 200;

                scope.Complete();
            }
            catch (Exception e)
            {
                problemResponse.Detail = e.Message;
                problemResponse.Status = 500;
                scope.Dispose();
            }

            return problemResponse;
        }

        public async Task<bool> EmailDuplicationCheck(string email)
        {
            var emailsCount = await _userManager.Users.Where(p => p.NormalizedEmail.Equals(email)).CountAsync();
            return emailsCount > 0;
        }

        private async Task<int> EmailCounter(string email)
        {
            return await _userManager.Users.Where(p => p.NormalizedEmail.Equals(email)).CountAsync();
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
            var user = await _userManager.FindByEmailAsync(userLogIn.Email);
            /*var user = _userManager.Users
                .SingleOrDefault(p => p.NormalizedEmail == userLogIn.Email.ToUpper());*/
            
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
        
        public async Task<ProblemResponse> EditUser(UserEdit userEdit, Guid userId, Guid authUserId, IEnumerable<string> authUserRoles)
        {
            ProblemResponse problemResponse = new ProblemResponse()
            {
                Detail = "Brak wymaganych uprawnień do edycji użytkownika",
                Status = 0
            };
            
            if (userId == authUserId || authUserRoles.Any(p => p == "Admin"))
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                try
                {
                    var user = await _userManager.FindByIdAsync(userId.ToString());

                    if (user is not null)
                    {
                        if (await EmailCounter(user.Email.ToUpper()) > 1)
                        {
                            throw new Exception("Podany adres e-mail już istnieje. Proszę podać inny");
                        }
                        
                        var setEmailResult = await _userManager.SetEmailAsync(user, userEdit.Email);
                        if (!setEmailResult.Succeeded)
                        {
                            var code = setEmailResult.Errors.FirstOrDefault().Code;
                            throw new Exception($"Problem przy zmianie adresu e-mail. Kod błędu: {code}");
                        }

                        var setUserNameResult = await _userManager.SetUserNameAsync(user, userEdit.UserName);
                        if (!setUserNameResult.Succeeded)
                        {
                            var code = setUserNameResult.Errors.FirstOrDefault().Code;
                            if (code is not null && code.Equals("DuplicateUserName."))
                            {
                                throw new Exception($"Podana nazwa użytkownika już istnieje. Proszę podać inną");                                
                            }
                            
                            throw new Exception($"Problem przy zmianie nazwy użytkownika. Kod błędu: {code}");
                        }

                        var setPhoneNumberResult =
                            await _userManager.SetPhoneNumberAsync(user, userEdit.PhoneNumber);
                        if (!setPhoneNumberResult.Succeeded)
                        {
                            var code = setPhoneNumberResult.Errors.FirstOrDefault().Code;
                            throw new Exception($"Problem przy zmianie numeru telefonu. Kod błędu: {code}");
                        }

                        if (userEdit.OldPassword is not null)
                        {
                            var changePasswordResult = await _userManager.ChangePasswordAsync(user,
                                userEdit.OldPassword,
                                userEdit.NewPassword);

                            if (!changePasswordResult.Succeeded)
                            {
                                var code = changePasswordResult.Errors.FirstOrDefault().Code;
                                if (code is not null && code.Equals("PasswordMismatch"))
                                {
                                    throw new Exception(
                                        $"Nieprawidłowe hasło. Podaj aktualne hasło dla wybranego użytkownika.");
                                }
                                
                                throw new Exception(
                                    $"Błąd podczas zmiany hasła. Kod błędu: {code}");
                            }
                        }

                        user.BasketId = userEdit.BasketId;
                        user.FundId = userEdit.FundId;
                        user.Debt = userEdit.Debt;
                        user.Info = userEdit.Info;
                        user.FirstName = userEdit.FirstName;
                        user.LastName = userEdit.LastName;

                        var updateUserResult = await _userManager.UpdateAsync(user);
                        if (!updateUserResult.Succeeded)
                        {
                            throw new Exception("Nie udało się zaktualizować danych");
                        }
                            
                        var currentUserRoles = await GetUserRoleAsync(userId.ToString());
                        var rolesToAdd = userEdit.Role.Where(p => !currentUserRoles.Contains(p));
                        var rolesToRemove = currentUserRoles.Where(p => !userEdit.Role.Contains(p));
                            
                        var addRolesResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                        if (!addRolesResult.Succeeded)
                        {
                            throw new Exception("Błąd w trakcie dodawania roli do użytkownika");
                        }

                        var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                        if (!removeRolesResult.Succeeded)
                        {
                            throw new Exception("Błąd w trakcie usuwania roli z użytkownika");
                        }
                    }

                    problemResponse.Detail = "Dane użytkownika zostały zaktualizowane";
                    problemResponse.Status = 200;

                    scope.Complete();
                }
                catch (Exception e)
                {
                    problemResponse.Detail = e.Message;
                    problemResponse.Status = 500;
                    scope.Dispose();
                }
            }

            return problemResponse;
        }
        
        public async Task<IdentityResult> RemoveUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return null;
            }

            return await _userManager.DeleteAsync(user);
        }

        public async Task<IEnumerable<Roles>> GetAllRolesAsync()
        {
            var rolesTmp = await _roleManager.Roles.ToListAsync();
            var roles = _mapper.Map<IEnumerable<Role>, IEnumerable<Roles>>(rolesTmp);
            return roles;
        }
        
        public Task<IdentityResult> CreateRole(string roleName)
        {
            var newRole = new Role() {Name = roleName};

            return _roleManager.CreateAsync(newRole);
        }

        public async Task<Guid> GetUserRoleId(string roleName)
        {
            var role = _roleManager.Roles.SingleOrDefault(p => p.Name.Equals(roleName));

            if (role is not null)
            {
                return role.Id;
            }
            
            return Guid.Empty;
        }

        public async Task<IList<string>> GetUserRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {
                return await _userManager.GetRolesAsync(user);
            }

            return null;
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

            IQueryable<User> usersGrouped;
            if (count > 0)
            {
                usersGrouped = usersSorted.Skip(start).Take(count);
            }
            else
            {
                usersGrouped = usersSorted.Skip(start);
            }

            var usersOutput = _mapper.Map<IEnumerable<User>, IEnumerable<UserEdit>>(usersGrouped);
            
            return usersOutput;
        }

        public async Task<ProblemResponse> GetPasswordResetTokenAsync(PasswordReset data)
        {
            ProblemResponse problemResponse = new ProblemResponse()
            {
                Detail = "Na podany adres mailowy został wysłany link do zresetowania hasła",
                Status = 200
            };

            try
            {
                Console.WriteLine($"Checking email: {data.Email}");
                var user = await _userManager.FindByEmailAsync(data.Email);
                if (user is null)
                {
                    problemResponse.Detail = "Na podany adres mailowy został wysłany link do zresetowania hasła";
                    throw new Exception("Na podany adres mailowy został wysłany link do zresetowania hasła");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                Console.WriteLine(code);
                string link = $"{data.HostName}/password-reset/new-email/{user.Id}/{HttpUtility.UrlEncode(code)}";
                problemResponse.Data = link;
                
                await _emailSender.SendPasswordResetToken(data.Email, link);
            }
            catch (Exception e)
            {
                problemResponse.Detail = e.Message;
                problemResponse.Status = 500;
            }

            return problemResponse;
        }

        public async Task<ProblemResponse> ResetPassword(PasswordReset data)
        {
            ProblemResponse problemResponse = new ProblemResponse()
            {
                Detail = "Coś poszło nie tak",
                Status = 500
            };

            try
            {
                Console.WriteLine(data.Token);
                var user = await _userManager.FindByIdAsync(data.UserId);
                if (user is null)
                {
                    throw new Exception("Użytkownik nie został znaleziony w bazie.");
                }

                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, data.Token, data.Password);
                if (!resetPasswordResult.Succeeded)
                {
                    var code = resetPasswordResult.Errors.FirstOrDefault().Code;
                    throw new Exception($"Coś poszło nie tak podczas resetowania hasła. Kod błędu: {code}");
                }
                
                problemResponse.Detail = "Hasło zostało pomyślnie zmienione";
                problemResponse.Status = 200;
            }
            catch (Exception e)
            {
                problemResponse.Detail = e.Message;
                problemResponse.Status = 500;
            }
            
            return problemResponse;
        }
        
        public async Task<ProblemResponse> SelfResetPassword(PasswordReset data)
        {
            ProblemResponse problemResponse = new ProblemResponse()
            {
                Detail = "Coś poszło nie tak",
                Status = 500
            };

            try
            {
                Console.WriteLine(data.Token);
                var user = await _userManager.FindByIdAsync(data.UserId);
                if (user is null)
                {
                    throw new Exception("Użytkownik nie został znaleziony w bazie.");
                }
                
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (token is null)
                {
                    throw new Exception("Problem przy tworzeniu tokenu do resetowania hasła.");
                }

                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, data.Password);
                if (!resetPasswordResult.Succeeded)
                {
                    var errCode = resetPasswordResult.Errors.FirstOrDefault().Code;
                    throw new Exception($"Coś poszło nie tak podczas resetowania hasła. Kod błędu: {errCode}");
                }
                
                problemResponse.Detail = "Hasło zostało pomyślnie zmienione";
                problemResponse.Status = 200;
            }
            catch (Exception e)
            {
                problemResponse.Detail = e.Message;
                problemResponse.Status = 500;
            }
            
            return problemResponse;
        }
    }
}
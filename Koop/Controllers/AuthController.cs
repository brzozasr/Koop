using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Koop.Models;
using Koop.Models.Auth;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Koop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ILogger<AuthController> _logger;
        
        private IGenericUnitOfWork _uow;

        public AuthController(IGenericUnitOfWork genericUnitOfWork, ILogger<AuthController> logger)
        {
            _uow = genericUnitOfWork;
            _logger = logger;
        }
        
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody]UserEdit newUser)
        {
            Console.WriteLine($"User first name: {newUser.FirstName}");
            var userCreateResult = await _uow.AuthService().SignUp(newUser);

            return Ok(userCreateResult);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody]UserLogIn userLogIn)
        {
            _logger.LogInformation(2, "Logging in {User}", userLogIn.Email);
            Response response = new Response();
            
            var refreshToken = await _uow.AuthService().SignIn(userLogIn);

            if (refreshToken is null)
            {
                return Problem("Nieprawidłowy login lub hasło.", null, 500);
            }

            try
            {
                _uow.SaveChanges();
                
                return Ok(refreshToken);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
        }

        [HttpPost("newRole/{roleName}")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleResult = await _uow.AuthService().CreateRole(roleName);
            
            if (roleResult.Succeeded)
            {
                return Ok();
            }

            return Problem(roleResult.Errors.First().Description, null, 500);
        }

        [HttpGet("user/{userId}/getRole")]
        public async Task<IActionResult> GetUserRole(string userId)
        {
            var result = await _uow.AuthService().GetUserRoleAsync(userId);

            result ??= new List<string>();

            // var roleId = await _uow.AuthService().GetUserRoleId(result);

            List<string> roles = new List<string>();
            foreach (var item in result)
            {
                roles.Add(item);
            }
            
            return Ok(roles);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _uow.AuthService().GetAllRolesAsync();
            
            return Ok(result);
        }

        [HttpPost("user/{userId}/addRole/{roleName}")]
        public async Task<IActionResult> AddRoleToUser(Guid userId, string roleName)
        {
            if (!_uow.DbContext.Roles.Any(p => p.NormalizedName.Equals(roleName.ToUpper())))
            {
                return Problem($"There is no such role like '{roleName}'.", null, 500);
            }
            
            var result = await _uow.AuthService().AddRoleToUser(userId, roleName);
            
            if (result.Succeeded)
            {
                return Ok();
            }
            
            return Problem(result.Errors.First().Description, null, 500);
        }
        
        [HttpDelete("user/{userId}/removeRole/{roleName}")]
        public async Task<IActionResult> RemoveRoleFromUser(Guid userId, string roleName)
        {
            var result = await _uow.AuthService().RemoveRoleFromUser(userId, roleName);
            
            if (result.Succeeded)
            {
                return Ok();
            }

            return Problem(result.Errors.First().Description, null, 500);
        }

        [HttpGet("user/{userId}/get")]
        public IActionResult GetUser(Guid userId)
        {
            var result = _uow.AuthService().GetUser(userId);

            if (result is null)
            {
                return Problem("Użytkownik nie istnieje", null, 500);
            }
            
            return Ok(result);
        }

        [Authorize]
        [HttpPost("user/{userId}/edit")]
        public async Task<IActionResult> EditUser(UserEdit userEdit, Guid userId)
        {
            var authUserId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            var authUserRoles = HttpContext.User.Claims.Where(p => p.Type == ClaimTypes.Role).Select(p => p.Value);
            
            if (authUserId is not null)
            {
                var result = await _uow.AuthService()
                    .EditUser(userEdit, userId, Guid.Parse(authUserId), authUserRoles);

                return Ok(result);
            }
            
            return Problem("User is not authenticated.", null, 500);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> GetPasswordResetTokenAsync(PasswordReset data)
        {
            var response = await _uow.AuthService().GetPasswordResetTokenAsync(data);

            return Ok(response);
        }
        
        [AllowAnonymous]
        [HttpPost("reset-password-set")]
        public async Task<IActionResult> ResetPassword(PasswordReset data)
        {
            var response = await _uow.AuthService().ResetPassword(data);

            return Ok(response);
        }
        
        [Authorize]
        [HttpPost("self-reset-password-set")]
        public async Task<IActionResult> SelfResetPassword(PasswordReset data)
        {
            var response = await _uow.AuthService().SelfResetPassword(data);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("user/{userId}/remove")]
        public async Task<IActionResult> RemoveUser(Guid userId)
        {
            Response.Headers.Add("Access-Control-Allow-Methods", "DELETE");
            try
            {
                var result = await _uow.AuthService().RemoveUser(userId);

                if (result is null)
                {
                    return Problem("Nie znaleziono użytkownika", null, 500);
                }
                
                if (result.Succeeded)
                {
                    return Ok(new {Detail = "Użytkownik został usunięty", Status = 200});
                }
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
            
            return Problem("Unknown error on the server side", null, 500);
        }

        [HttpPost("user/refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var refreshToken = HttpContext.Request.Headers["TokenRefresh"].ToString();

            Console.WriteLine($"Token: {token}");
            Console.WriteLine($"RefreshToken: {refreshToken}");
            
            ClaimsPrincipal principal;
            try
            {
                principal = _uow.AuthService().GetPrincipalFromExpiredToken(token);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 500);
            }
            
            // var userId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            var userId = principal.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Problem("Could not get the user Id from the token.", null, 401);
            }
            
            /*string refreshTokenFromHeader = "";
            if (HttpContext.Request.Cookies.ContainsKey("refresh_token"))
            {
                Console.WriteLine($"Cookie: {HttpContext.Request.Cookies["refresh_token"]}");
                refreshTokenFromHeader = HttpContext.Request.Cookies["refresh_token"];
            }
            else
            {
                return Problem("Could not get the refresh token from the header.", null, 401);
            }*/

            var user = await _uow.AuthService().GetUserRaw(Guid.Parse(userId));
            
            /*var refreshToken = _uow.Repository<RefreshToken>().GetAll()
                .SingleOrDefault(p => p.UserId == Guid.Parse(userId));*/

            if (user is null)
            {
                return Problem("Could not get the user from the database.", null, 401);
            }

            if (user.RefreshTokenExp < DateTime.Now)
            {
                return Problem("Refresh token was expired.", null, 401);
            }
            
            if (user.RefreshToken.Equals(refreshToken))
            {
                var response = await _uow.AuthService().GetNewToken(Guid.Parse(userId));
                
                try
                {
                    _uow.SaveChanges();
                
                    return Ok(response);
                }
                catch (Exception e)
                {
                    return Problem(e.Message, null, 500);
                }
            }
            
            return Problem("Refresh token from the header is not equal to the token in the database.", null, 401);
        }

        [HttpGet("user/all")]
        public IActionResult GetAllUsers(string orderBy = "lastName", int start = 0, int count = 0, string orderDir = "asc")
        {
            orderBy = orderBy.ToLower();
            
            Expression<Func<User, object>> order = orderBy switch
            {
                "firstName" => p => p.FirstName,
                "lastName" => p => p.LastName,
                "email" => p => p.Email,
                _ => p => p.LastName
            };
            
            OrderDirection direction = orderDir switch
            {
                "asc" => OrderDirection.Asc,
                "desc" => OrderDirection.Desc,
                _ => OrderDirection.Asc
            };

            var result = _uow.AuthService().GetAllUsers(order, start, count, direction);
            
            return Ok(result);
            //return Ok(_uow.ShopRepository().GetProductsShop(Guid.Parse(userId), order, start, count, direction));
        }

        [HttpGet("user/email/check")]
        public async Task<IActionResult> EmailDuplicationCheck(string email)
        {
            Console.WriteLine($"Email: {email}");
            try
            {
                var result = await _uow.AuthService().EmailDuplicationCheck(email.ToUpper());

                var output = new
                {
                    Result = result
                };

                return Ok(output);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
        
        [HttpGet("user/username/check")]
        public async Task<IActionResult> UsernameDuplicationCheck(string username)
        {
            try
            {
                var result = await _uow.AuthService().UserDuplicationCheck(username.ToUpper());

                var output = new
                {
                    Result = result
                };

                return Ok(output);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
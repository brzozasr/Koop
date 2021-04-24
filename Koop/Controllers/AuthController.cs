using System;
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

            if (userCreateResult is null)
            {
                return Problem("User with the same email already exists.", null, 500);
            }
            
            if (userCreateResult.Succeeded)
            {
                return Created(string.Empty, string.Empty);
            }

            // return Problem(userCreateResult.Errors.ToString(), null, 500);
            return BadRequest(userCreateResult.Errors);
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
            return Ok(_uow.AuthService().GetUser(userId));
        }

        [Authorize]
        [HttpPost("user/{userId}/edit")]
        public async Task<IActionResult> EditUser(UserEdit userEdit, Guid userId)
        {
            var authUserId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            var authUserRoles = HttpContext.User.Claims.Where(p => p.Type == ClaimTypes.Role).Select(p => p.Value);
            
            if (authUserId is not null)
            {
                var result = await _uow.AuthService().EditUser(userEdit, userId, Guid.Parse(authUserId), authUserRoles);

                if (result is null)
                {
                    return Problem("Not enough privileges to edit user's credentials.", null, 500);
                }
                
                if (result.Succeeded)
                {
                    return Ok(userEdit);
                }
                
                return Problem(result.Errors.First().Description, null, 500);
            }
            
            return Problem("User is not authenticated.", null, 500);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("user/{userId}/remove")]
        public async Task<IActionResult> RemoveUser(Guid userId)
        {
            var result = await _uow.AuthService().RemoveUser(userId);

            if (result.Succeeded)
            {
                return Ok(new ShopRepositoryResponse() {Message = "User removed.", StatusCode = 200});
            }
            
            return Problem(result.Errors.First().Description, null, 500); 
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
        public IActionResult GetAllUsers(string orderBy = "lastName", int start = 0, int count = 10, string orderDir = "asc")
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
    }
}
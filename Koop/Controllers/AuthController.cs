using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Koop.Models.Auth;
using Koop.Models.Repositories;
using Koop.Models.RepositoryModels;
using Koop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IGenericUnitOfWork _uow;

        public AuthController(IGenericUnitOfWork genericUnitOfWork)
        {
            _uow = genericUnitOfWork;
        }
        
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody]UserEdit newUser)
        {
            var userCreateResult = await _uow.AuthService().SignUp(newUser);
            
            if (userCreateResult.Succeeded)
            {
                return Created(string.Empty, string.Empty);
            }

            // return Problem(userCreateResult.Errors.ToString(), null, 500);
            return BadRequest(userCreateResult.Errors);
        }

        [HttpPost("signin")]
        public IActionResult SignIn(UserLogIn userLogIn)
        {
            var token = _uow.AuthService().SignIn(userLogIn);

            if (token is null)
            {
                return BadRequest("Email or password incorrect");
            }

            return Ok(token);
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
    }
}
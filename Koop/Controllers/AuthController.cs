using System;
using System.Linq;
using System.Threading.Tasks;
using Koop.Models.Auth;
using Koop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Koop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody]UserSignUp userSignUp)
        {
            var userCreateResult = await _authService.SignUp(userSignUp);
            
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
            var token = _authService.SignIn(userLogIn);

            if (token is null)
            {
                return BadRequest("Email or password incorrect");
            }

            return Ok(token);
        }

        [HttpPost("newrole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleResult = await _authService.CreateRole(roleName);
            
            if (roleResult.Succeeded)
            {
                return Ok();
            }

            return Problem(roleResult.Errors.First().Description, null, 500);
        }

        [HttpPost("user/addrole")]
        public async Task<IActionResult> AddUserToRole(UserAddRole userAddRole)
        {
            var result = await _authService.AddUserToRole(userAddRole.Id, userAddRole.RoleName);
            
            if (result.Succeeded)
            {
                return Ok();
            }

            return Problem(result.Errors.First().Description, null, 500);
        }
    }
}
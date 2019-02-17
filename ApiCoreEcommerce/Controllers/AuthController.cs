using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiCoreEcommerce.Dtos.Requests;
using ApiCoreEcommerce.Dtos.Requests.Users;
using ApiCoreEcommerce.Dtos.Responses.Shared;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Models;
using ApiCoreEcommerce.Services;
using ApiCoreEcommerce.Services.Interfaces;
using BlogDotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BlogDotNet.Controllers
{
    
    [Route("api/")]
    public class AuthController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(IUsersService usersService, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _usersService = usersService;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("users")]
        public IActionResult Get()
        {
            return Ok(new
            {
                Success = true,
                Message = "Done"
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return StatusCodeAndDtoWrapper.BuildSuccess(new ErrorDtoResponse
                {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [HttpGet("2")]
        public IActionResult Get2()
        {
            return StatusCode(200, "Ok");
        }

        [HttpGet("Authorized")]
        [Authorize]
        public IActionResult Get3()
        {
            return StatusCode(200, User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [HttpPost("Register")]
        [Route("users")]
        [Route("auth/register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (!ModelState.IsValid)
                return StatusCodeAndDtoWrapper.BuilBadRequest(ModelState);
            var user = new ApplicationUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await _usersService.Create(user, registerDto.Password);

            if (result.Succeeded)
            {
                if (result.Succeeded)
                {
                    result = await _usersService.AddToRoleAsync(user, "ROLE_USER");
                    if (result.Succeeded)
                    {
                        return StatusCodeAndDtoWrapper.BuildSuccess("Registered successfully");
                    }
                    else
                    {
                        return StatusCodeAndDtoWrapper.BuildBadRequest(result.Errors);
                    }
                }
                else
                {
                    return StatusCodeAndDtoWrapper.BuildBadRequest(result.Errors);
                }
            }
            else
                return StatusCodeAndDtoWrapper.BuildBadRequest(result.Errors);
        }

        [HttpPost("Login")]
        [Route("users/login")]
        [Route("auth/login")]
        public async Task<object> Login([FromBody] LoginDtoRequest loginDto)
        {
            // Sign in the user, don't persis cookies, don't lockout on failure
            var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password,
                false, false);

            if (result.Succeeded)
            {
                ApplicationUser user = await _usersService.GetByUserNameAsync(loginDto.UserName);
                return await GenerateJwtToken(user);
            }
            else
            {
                return StatusCodeAndDtoWrapper.BuildErrorResponse("Invalid credentials");
            }
        }

        private async Task<object> GenerateJwtToken(IdentityUser<long> user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWT_SUPER_SECRET"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var token = new JwtSecurityToken(
                issuer: "",
                claims: new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    // new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

/*new Claim(JwtRegisteredClaimNames.Iat, 
    new DateTimeOffset(_jwtOptions.IssuedAt).ToUnixTimeSeconds().ToString(),
    ClaimValueTypes.Integer64)
    */
                },
                expires: DateTime.Now.AddDays(2),
                signingCredentials: creds);

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    id = user.Id,
                    username = user.UserName,
                    token = tokenStr,
                    roles = (await _usersService.GetUserRolesAsync((ApplicationUser) user))
                },
            });
        }
    }
}
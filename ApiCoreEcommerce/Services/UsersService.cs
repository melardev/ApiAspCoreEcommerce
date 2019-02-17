using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiCoreEcommerce.Services
{
    public class UsersService : IUsersService
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;
        private readonly SignInManager<ApplicationUser> _signInManager;


        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IUserValidator<ApplicationUser> _userValidator;

        private readonly IConfigurationService _configurationService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private HtmlEncoder _htmlEncoder;

        public UsersService(UserManager<ApplicationUser> userManager,
            IConfigurationService configurationService, IUserValidator<ApplicationUser> userValidator,
            SignInManager<ApplicationUser> signInManager,
            IPasswordValidator<ApplicationUser> passwordValidator,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IHttpContextAccessor httpContextAccessor,
            HtmlEncoder htmlEncoder
        )
        {
            _userManager = userManager;
            _configurationService = configurationService;
            _userValidator = userValidator;
            _signInManager = signInManager;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _htmlEncoder = htmlEncoder;
        }


        public async Task<List<ApplicationUser>> FetchAll()
        {
            return await _userManager.Users.ToListAsync();
        }

        public string GetCurrentUserId()
        {
            // instead of FindFirst().Value we can use FindFirstValue
            string currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return currentUserId;
        }

        public long GetUserId(ClaimsPrincipal user)
        {
            // ControllerBase has User, so you can use from any controller ( GetUserId(User) )
            return Convert.ToInt64(user.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        }

        public async Task<ApplicationUser> GetByIdAsync(long id)
        {
            //return await _userManager.Users.Where(user => user.Id == id).FirstOrDefaultAsync();
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<ApplicationUser> GetByPrincipal(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

        //GetRolesAsync(user) Returns a list of the names of the roles of which the user is a member
        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            //Returns a list of the names of the roles of which the user is a member
            return await _userManager.GetRolesAsync(user);
        }


        public async Task<ApplicationUser> GetByUserNameAsync(string username)
        {
            //_userManager.Users.SingleOrDefault(u => u.Email == username)
            ApplicationUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));
            return user;
        }

        public async Task<bool> IsAdmin()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            if (user == null)
                return false;
            return await IsUserInRoleAsync(user, _configurationService.GetAdminRoleName());
        }

        public async Task<bool> IsUserInRoleAsync(ApplicationUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<bool> IsUserInRole(int userId, string roleName)
        {
            //var user = await GetCurrentUserAsync();
            return await IsUserInRoleAsync(await GetByIdAsync(userId), roleName);
        }

        public async Task<bool> IsUserInRole(ApplicationUser user, string roleName)
        {
            return await IsUserInRoleAsync(user, roleName);
        }

        public async Task<bool> IsUserInRole(string roleName)
        {
            var user = await GetCurrentUserAsync();
            return await IsUserInRoleAsync(user, roleName);
        }

        public bool IsUserInRole(ClaimsPrincipal user, string roleName)
        {
            return user.IsInRole(roleName);
        }

        public bool IsCurrentUserLoggedIn()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity.IsAuthenticated == true;
        }

        public bool IsUserLoggedIn(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }

        public bool IsUserLoggedIn(IIdentity user)
        {
            return user != null && user.IsAuthenticated;
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
        {
            //AddToRoleAsync(user, name) Adds the user ID to the role with the specified name
            //RemoveFromRoleAsync(user, name)
            return await _userManager.AddToRoleAsync(user, roleName);
        }


        public async Task<IdentityResult> Create(ApplicationUser user, string password)
        {
            user.UserName = _htmlEncoder.Encode(user.UserName);
            user.FirstName = _htmlEncoder.Encode(user.FirstName);
            user.LastName = _htmlEncoder.Encode(user.LastName);
            user.Email = _htmlEncoder.Encode(user.Email);

            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> Create(string userName, string firstName, string lastName,
            string email,
            string password)
        {
            var user = new ApplicationUser
            {
                UserName = _htmlEncoder.Encode(userName),
                FirstName = _htmlEncoder.Encode(firstName),
                LastName = _htmlEncoder.Encode(lastName),
                Email = _htmlEncoder.Encode(email),
            };
            return await _userManager.CreateAsync(user, password);
        }


        public Task<bool> CheckPasswordAgainstPolicies(string password)
        {
            // Check if password complies our defined password policy
            //var task = _userManager.PasswordValidators[0].ValidateAsync(password);
            return Task.FromResult(true);
        }

        public async Task<bool> CheckPasswordValid(string password, ApplicationUser user)
        {
            foreach (var userManagerPasswordValidator in _userManager.PasswordValidators)
            {
                IdentityResult res = await userManagerPasswordValidator.ValidateAsync(_userManager, user, password);
                if (res.Succeeded)
                    return true;
            }

            return false;
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword,
            string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }


        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> Delete(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }
    }
}
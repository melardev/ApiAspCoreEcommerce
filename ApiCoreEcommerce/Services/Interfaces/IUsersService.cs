using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ApiCoreEcommerce.Entities;
using Microsoft.AspNetCore.Identity;

namespace ApiCoreEcommerce.Services.Interfaces
{
    public interface IUsersService
    {
        string GetCurrentUserId();
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<bool> IsUserInRole(int userId, string roleName);
        bool IsUserInRole(ClaimsPrincipal user, string roleName);
        Task<ApplicationUser> GetByPrincipal(ClaimsPrincipal principal);

        Task<ApplicationUser> GetCurrentUserAsync();

        Task<ApplicationUser> GetByIdAsync(long id);
        bool IsCurrentUserLoggedIn();
        bool IsUserLoggedIn(ClaimsPrincipal user);
        Task<bool> IsUserInRole(ApplicationUser user, string roleName);
        bool IsUserLoggedIn(IIdentity user);
        Task<IdentityResult> Create(ApplicationUser user, string password);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName);

        Task<IdentityResult> Create(string userName,
            string firstName, string lastName,
            string email, string password);

        Task<IdentityResult> Delete(ApplicationUser user);
        Task<ApplicationUser> GetByUserNameAsync(string username);

        Task<bool> IsAdmin();

        Task<bool> IsUserInRoleAsync(ApplicationUser user, string roleName);
    }
}
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ApiCoreEcommerce.Entities
{
    public class ApplicationUser : IdentityUser<long>
    {
        public ApplicationUser()
        {
            // Orders = new HashSet<Order>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<AppUserRole> Roles { get; set; } = new List<AppUserRole>();


        //public virtual ICollection<IdentityUserRole<int>> Roless { get; } = new List<IdentityUserRole<int>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<long>> Claims { get; } = new List<IdentityUserClaim<long>>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin<long>> Logins { get; } = new List<IdentityUserLogin<long>>();

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public ICollection<Rating> Ratings { get; set; }
    }
}
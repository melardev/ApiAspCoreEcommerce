using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApiCoreEcommerce.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ApiCoreEcommerce.Dtos.Requests
{
    public class RegisterUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "password_confirmation")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string PasswordConfirmation { get; set; }

        //testing
        [Display(Name = "User Role")] public List<SelectListItem> UserRoles { get; set; }
        public ApplicationRole Role { get; set; }
        public string RoleName { get; set; }
    }
}
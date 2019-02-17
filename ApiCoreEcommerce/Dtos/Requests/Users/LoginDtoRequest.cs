using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ApiCoreEcommerce.Dtos.Requests.Users
{
    public class LoginDtoRequest
    {
        [Required]
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [Required]
        [JsonProperty(PropertyName = "password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

/*
        [Required] [EmailAddress] public string Email { get; set; }
        [Display(Name = "Remember me?")] public bool RememberMe { get; set; }
        */
    }
}
using System;
using ApiCoreEcommerce.Enums;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ApiCoreEcommerce.Services
{
    public class ConfigurationService : IConfigurationService
    {
         private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetAdminUserName()
        {
            //_configuration.GetSection("Users")["Admin::Username"];
            // Coalescense expression
            return _configuration["Auth::Admin::Username"] ?? "admin";
        }

        public string GetAdminEmail()
        {
            return _configuration["Auth::Admin::Email"] ?? "admin@no-reply.com";
        }

        public string GetAdminRoleName()
        {
            return _configuration["Auth::Roles::Admin"] ?? "ROLE_ADMIN";
        }

        public string GetAdminPassword()
        {
            return _configuration["Auth::Admin::Password"] ?? "password";
        }

        public int GetDefaultPage()
        {
            return Convert.ToInt32(_configuration["Content::Page::First"] ?? "1");
        }

        public int GetDefaultPageSize()
        {
            return Convert.ToInt32(_configuration["Content::Page::Size"] ?? "5");
        }

        public string GetManageProductPolicyName()
        {
            return _configuration["Auth::Policies:Products::Edit::Name"] ?? "ManageProductsPolicy";
        }

        public string GetCreateCommentPolicyName()
        {
            return _configuration["Auth::Policies:Comments::Create::Name"] ?? "CreateCommentsPolicy";
        }

        public string GetDeleteCommentPolicyName()
        {
            return _configuration["Auth::Policies:Comments::Delete::Name"] ?? "DeleteCommentsPolicy";
        }

        public string GetWhoIsAllowedToManageProducts()
        {
            return GetAdminRoleName();
            // return Enum.Parse<AuthorizationPolicy>(_configuration["Auth::Policies:Products::Create::Who"] ?? "AUTHENTICATED_USER");
        }


        public string GetWhoIsAllowedToCreateComments()
        {
            return GetStandardUserRoleName();
            // return Enum.Parse<AuthorizationPolicy>(_configuration["Auth::Policies:Comment::Edit::Who"] ??      "AUTHENTICATED_USER");
        }

        public AuthorizationPolicy GetWhoIsAllowedToDeleteComments()
        {
            return (AuthorizationPolicy) Enum.Parse(typeof(AuthorizationPolicy),
                _configuration["Auth::Policies:Comment::Delete::Who"] ??
                AuthorizationPolicy.ADMIN_AND_OWNER.ToString());
        }

        public AuthorizationPolicy GetWhoIsAllowedToUpdateComments()
        {
            return (AuthorizationPolicy) Enum.Parse(typeof(AuthorizationPolicy),
                _configuration["Auth::Policies:Comment::Update::Who"] ??
                AuthorizationPolicy.ADMIN_AND_OWNER.ToString());
        }

        public string GetStandardUserRoleName()
        {
            return "ROLE_USER";
        }

        public string GetAdminFirstName()
        {
            return "adminFN";
        }

        public string GetAdminLastName()
        {
            return "adminLN";
        }

        public string GetUpdateCommentPolicyName()
        {
            return _configuration["Auth::Policies:Comments::Delete::Name"] ?? "UpdateCommentsPolicy";
        }
    }
}
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ApiCoreEcommerce.Data;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Infrastructure.Handlers;
using ApiCoreEcommerce.Services;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ApiCoreEcommerce.Infrastructure.Extensions
{
    public static class AuthExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>();

            //services.AddIdentity<IdentityUser, ApplicationRole>()
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;

                    // options.SignIn.RequireConfirmedEmail = true;

                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            // ===== Add Jwt Authentication ========
            var issuer = configuration["Security:Jwt:JwtIssuer"];
            var audience = configuration.GetSection("Security:Jwt:JwtIssuer").Value;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = issuer, //configuration["Security::Jwt::JwtIssuer"],
                        ValidAudience = audience, //configuration["Security::Jwt::JwtAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWT_SUPER_SECRET")),
                        //new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Security::Jwt::JwtKey"])),
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });
        }

        public static void AddAppAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationService settingsService =
                services.BuildServiceProvider().GetRequiredService<IConfigurationService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(settingsService.GetManageProductPolicyName(), policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(
                        new ResourceAuthorizationHandler.AllowedToManageProductRequirement(settingsService
                            .GetWhoIsAllowedToManageProducts()));
                });


                options.AddPolicy(settingsService.GetCreateCommentPolicyName(), policy =>
                {
                    policy.Requirements.Add(
                        new ResourceAuthorizationHandler.AllowedToCreateCommentRequirement(settingsService
                            .GetWhoIsAllowedToCreateComments()));
                });


                options.AddPolicy(settingsService.GetUpdateCommentPolicyName(), policy =>
                {
                    policy.Requirements.Add(
                        new ResourceAuthorizationHandler.AllowedToUpdateCommentRequirement(settingsService
                            .GetWhoIsAllowedToUpdateComments()));
                });


                options.AddPolicy(settingsService.GetDeleteCommentPolicyName(), policy =>
                {
                    policy.Requirements.Add(
                        new ResourceAuthorizationHandler.AllowedToDeleteCommentRequirement(settingsService
                            .GetWhoIsAllowedToDeleteComments()));
                });
            });

            // CORS
            services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                }
            );
        }

        public static void AddIdentityAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    //Configuration.GetConnectionString("DataSources::Sqlite::ConnectionString")
                    configuration.GetSection("DataSources:Sqlite:ConnectionString").Value
                ));
            // options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            /*services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(connectionString))
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();*/
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            /*
             * services.AddIdentityCore<IdentityUser, ApplicationRole>(options => {
                options.User.RequireUniqueEmail = true
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

             */

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/users/login";
                    options.LogoutPath = "/logout";
                });
        }


        private static void ConfigureAuth(IServiceCollection services)
        {
            /*
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => { options.TokenValidationParameters = _tokenValidationParameters; })
                .AddCookie(options =>
                {
                    options.Cookie.Name = Configuration.GetSection("TokenAuthentication:CookieName").Value;
                    options.TicketDataFormat = new CustomJwtDataFormat(
                        SecurityAlgorithms.HmacSha256,
                        _tokenValidationParameters);
                });
            
            */
        }

        public static void Startup(IHostingEnvironment env)
        {
/*
            _signingKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(Configuration.GetSection("TokenAuthentication:SecretKey").Value));

            _tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                // Validate the token expiry
                ValidateLifetime = true,
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };


            _tokenProviderOptions = new TokenProviderOptions
            {
                Path = Configuration.GetSection("TokenAuthentication:TokenPath").Value,
                Audience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                Issuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256),
                IdentityResolver = GetIdentity
            };
             */
        }
    }
}
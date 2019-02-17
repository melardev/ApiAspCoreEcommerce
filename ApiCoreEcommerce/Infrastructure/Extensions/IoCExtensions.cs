using ApiCoreEcommerce.Infrastructure.Handlers;
using ApiCoreEcommerce.Services;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCoreEcommerce.Infrastructure.Extensions
{
    public static class IoCExtensions
    {
        public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IProductsService, ProductsService>();
            services.AddTransient<ICommentsService, CommentsService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IAddressesService, AddressesService>();
            services.AddTransient<IOrdersService, OrderService>();
            services.AddTransient<ITagsService, TagsService>();
            services.AddTransient<ICategoriesService, CategoriesService>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IStorageService, StorageService>();
            services.AddTransient<IAuthorizationHandler, ResourceAuthorizationHandler>();
        }
    }
}
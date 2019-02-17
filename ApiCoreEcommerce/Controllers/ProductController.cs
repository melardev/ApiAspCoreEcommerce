using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApiCoreEcommerce.Dtos.Responses;
using ApiCoreEcommerce.Dtos.Responses.Products;
using ApiCoreEcommerce.Dtos.Responses.Shared;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Models;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCoreEcommerce.Controllers
{
    [Route("/api/products")]
    public class ProductController : Controller
    {
        public int PageSize = 4;


        private readonly IProductsService _productsService;
        private readonly IConfigurationService _configurationService;
        private readonly IAuthorizationService _authorizationService;

        private readonly IUsersService _usersService;
        //private MailService _mailService;

        public ProductController(IConfigurationService settingsService,
            IAuthorizationService authorizationService, IProductsService productService,
            IConfigurationService configurationService, IUsersService usersService)
        {
            _productsService = productService;
            _configurationService = configurationService;
            _usersService = usersService;
            _authorizationService = authorizationService;
        }


        [HttpGet("")]
        [ActionName(nameof(Index))]
        public async Task<IActionResult> Index(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var products = await _productsService.FetchPage(page, pageSize);
            var basePath = Request.Path;

            return StatusCodeAndDtoWrapper.BuildSuccess(ProductListDtoResponse.Build(products.Item2, basePath,
                currentPage: page, pageSize: pageSize, totalItemCount: products.Item1));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(string name, string description, int price, int stock,
            List<IFormFile> images)
        {
            if (!(await _usersService.IsAdmin()))
                return StatusCodeAndDtoWrapper.BuildUnauthorized("Only admin user can create prodcuts");

            // If the user sends `images` POST param then the list<IFormFile> will be populated, if the user sends `images[]` instead, then it will be empty
            // this is why I populate that list with this little trick
            if (images?.Count == 0)
                images = Request.Form.Files.GetFiles("images[]").ToList();

            List<Tag> tags = new List<Tag>();
            List<Category> categories = new List<Category>();

            foreach (string formKey in Request.Form.Keys)
            {
                Regex regex = new Regex("tags|categories\\[(?<name>\\w+)\\]");
                Match match = regex.Match(formKey);
                if (match.Success && formKey.StartsWith("tag"))
                {
                    var tagName = match.Groups["name"].Value;
                    tags.Add(new Tag
                    {
                        Name = tagName,
                        Description = Request.Form[key: formKey].ToString()
                    });
                }

                if (match.Success && formKey.StartsWith("cate"))
                {
                    var categoryName = match.Groups["name"].Value;
                    categories.Add(new Category
                    {
                        Name = categoryName,
                        Description = Request.Form[key: formKey].ToString()
                    });
                }
            }


            Product product = await _productsService.Create(name, description, price, stock, tags, categories, images);
            return StatusCodeAndDtoWrapper.BuildSuccess(ProductDetailsDto.Build(product));
        }

        [HttpPut("{slug}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(string slug, [FromBody] CreateOrEditProduct dto)
        {
            if (!ModelState.IsValid)
                return StatusCodeAndDtoWrapper.BuilBadRequest(ModelState);


            Product product = await _productsService.Update(slug, dto);

            return StatusCodeAndDtoWrapper.BuildSuccess(ProductDetailsDto.Build(product), "Updated successfully");
        }


        [HttpGet("search/{term}")]
        public async Task<IActionResult> GetBySearchTerm(string term, int page = 1, int pageSize = 5)
        {
            Tuple<int, List<Product>> result;
            if (!string.IsNullOrEmpty(term))
            {
                result = await _productsService.FetchBySearchTerm(term, page, pageSize);
            }
            else
                result = await _productsService.FetchPage(page, pageSize);

            return StatusCodeAndDtoWrapper.BuildSuccess(ProductListDtoResponse.Build(result.Item2, "search/", page,
                pageSize, result.Item1));
        }


        [Route("/by_category/{category}")]
        public async Task<IActionResult> GetByCategory([FromRoute] string category, int page = 1, int pageSize = 5)
        {
            Tuple<int, List<Product>> products = await _productsService.FetchPageByCategory(category, pageSize, page);

            return StatusCodeAndDtoWrapper.BuildSuccess(ProductListDtoResponse.Build(products.Item2,
                "/by_category/{category}", page, pageSize, products.Item1));
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetProductBySlug(string slug)
        {
            var product = await _productsService.GetProductBySlug(slug);
            if (product == null)
                return StatusCodeAndDtoWrapper.BuildNotFound(new ErrorDtoResponse("Not Found"));

            return new StatusCodeAndDtoWrapper(ProductDetailsDto.Build(product));
        }

        [HttpGet("by_id/{id}")]
        public async Task<IActionResult> GetProductById(long id)
        {
            var product = await _productsService.FetchById(id);
            if (product == null)
                return StatusCodeAndDtoWrapper.BuildNotFound(new ErrorDtoResponse("Not Found"));

            return StatusCodeAndDtoWrapper.BuildGeneric(ProductDetailsDto.Build(product));
        }


        [HttpDelete]
        [Authorize]
        [Route("products/{slug}")]
        [Route("products/by_id/{id}")]
        public async Task<IActionResult> Delete(long? id, string slug)
        {
            Product product;
            if (id != null)
                product = await _productsService.FetchById(id.Value);
            else
                product = await _productsService.FetchBySlug(slug);

            if (product == null)
            {
                return StatusCodeAndDtoWrapper.BuildGenericNotFound();
            }

            var result = await _authorizationService.AuthorizeAsync(User, product,
                _configurationService.GetManageProductPolicyName());
            if (result.Succeeded)
            {
                if ((await _productsService.Delete(product)) > 0)
                {
                    return StatusCodeAndDtoWrapper.BuildSuccess("Product deleted successfully");
                }
                else
                {
                    return StatusCodeAndDtoWrapper.BuildErrorResponse("An error occured, try later");
                }
            }
            else
            {
                return StatusCodeAndDtoWrapper.BuildUnauthorized("Access denied");
            }
        }
    }
}
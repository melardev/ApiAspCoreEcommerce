using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiCoreEcommerce.Dtos.Responses.Category;
using ApiCoreEcommerce.Dtos.Responses.Products;
using ApiCoreEcommerce.Dtos.Responses.Tag;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Models;
using ApiCoreEcommerce.Services;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCoreEcommerce.Controllers
{
    [Route("/api")]
    public class TagAndCategoryController : Controller
    {
        private readonly ITagsService _tagsService;
        private readonly IUsersService _usersService;
        private readonly ICategoriesService _categoriesService;

        public TagAndCategoryController(ITagsService tagsService, IUsersService usersService, ICategoriesService categoriesService)
        {
            this._tagsService = tagsService;
            _usersService = usersService;
            _categoriesService = categoriesService;
        }

        [HttpGet]
        [Route("tags")]
        public async Task<IActionResult> GetTags([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var tags = await _tagsService.FetchPage(page, pageSize);
            var basePath = Request.Path;

            return StatusCodeAndDtoWrapper.BuildSuccess(TagListDtoResponse.Build(tags.Item2, basePath,
                currentPage: page, pageSize: pageSize, totalItemCount: tags.Item1));
        }

        [HttpPost]
        [Route("tags")]
        public async Task<IActionResult> CreateTag(string name, string description, List<IFormFile> images)
        {
            // If the user sends `images` POST param then the list<IFormFile> will be populated, if the user sends `images[]` instead, then it will be empty
            // this is why I populate that list with this little trick
            if (images?.Count == 0)
                images = Request.Form.Files.GetFiles("images[]").ToList();
            Tag tag = await _tagsService.Create(name, description, images);
            return StatusCodeAndDtoWrapper.BuildSuccess(TagDto.Build(tag), "Tag created successfully");
        }
        
        
        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var categories = await _categoriesService.FetchPage(page, pageSize);
            var basePath = Request.Path;

            return StatusCodeAndDtoWrapper.BuildSuccess(CategoryListDtoResponse.Build(categories.Item2, basePath,
                currentPage: page, pageSize: pageSize, totalItemCount: categories.Item1));
        }

        [HttpPost]
        [Route("categories")]
        public async Task<IActionResult> CreateCategory(string name, string description, List<IFormFile> images)
        {
            
            // If the user sends `images` POST param then the list<IFormFile> will be populated, if the user sends `images[]` instead, then it will be empty
            // this is why I populate that list with this little trick
            if (images?.Count == 0)
                images = Request.Form.Files.GetFiles("images[]").ToList();
            
            Category category = await _categoriesService.Create(name, description, images, Convert.ToInt64(_usersService.GetCurrentUserId()));
            return StatusCodeAndDtoWrapper.BuildSuccess(CategoryDto.Build(category), "Category created successfully");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiCoreEcommerce.Dtos.Responses;
using ApiCoreEcommerce.Dtos.Responses.Pages;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Models;
using ApiCoreEcommerce.Services;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiCoreEcommerce.Controllers
{
    [Route("/api")]
    public class HomeController : Controller
    {
        private readonly IProductsService _productsService;
        private readonly ITagsService _tagsService;
        private readonly ICategoriesService _categoriesService;

        public HomeController(IProductsService productService, ITagsService tagsService,
            ICategoriesService categoriesService)
        {
            _productsService = productService;
            _tagsService = tagsService;
            _categoriesService = categoriesService;
        }

        
        [HttpGet]
        [Route("")]
        [Route("home")]
        public async Task<IActionResult> Index()
        {
            Tuple<int, List<Tag>> tags = await _tagsService.FetchPageWithImages(1, 3);
            Tuple<int, List<Category>> categories = await _categoriesService.FetchPageWithImages(1, 3);

            return StatusCodeAndDtoWrapper.BuildSuccess(HomeResponse.Build(tags.Item2, categories.Item2));
        }
    }
}
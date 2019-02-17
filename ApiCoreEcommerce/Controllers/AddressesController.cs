using System.Threading.Tasks;
using ApiCoreEcommerce.Dtos.Responses.Products;
using ApiCoreEcommerce.Models;
using ApiCoreEcommerce.Services;
using ApiCoreEcommerce.Services.Interfaces;
using BlogDotNet.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Models.AccountViewModels.Requests.Addresses;

namespace ApiCoreEcommerce.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class AddressesController : Controller
    {
        private readonly IAddressesService _addressesService;
        private readonly IUsersService _usersService;

        public AddressesController(IAddressesService addressesService, IUsersService usersService)
        {
            this._addressesService = addressesService;
            this._usersService = usersService;
        }

        [Route("addresses")]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var addresses =
                await _addressesService.FetchPageByUser(await _usersService.GetCurrentUserAsync(), page,
                    pageSize);
            var basePath = Request.Path;

            return StatusCodeAndDtoWrapper.BuildSuccess(AddressesListDtoResponse.Build(addresses.Item2, basePath,
                currentPage: page, pageSize: pageSize, totalItemCount: addresses.Item1));
        }


        [Route("addresses")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrEditAddressDto dto)
        {
            var address = await _addressesService.Create(await _usersService.GetCurrentUserAsync(), dto.FirstName,
                dto.Lastname, dto.Country, dto.City, dto.StreetAddress, dto.ZipCode);


            return StatusCodeAndDtoWrapper.BuildSuccess(AddressDto.Build(address));
        }
    }
}
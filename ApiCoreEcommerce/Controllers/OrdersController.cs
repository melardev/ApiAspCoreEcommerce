using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiCoreEcommerce.Dtos.Requests.Orders;
using ApiCoreEcommerce.Dtos.Responses.Orders;
using ApiCoreEcommerce.Dtos.Responses.Shared;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Models;
using ApiCoreEcommerce.Services;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCoreEcommerce.Controllers
{
    [Route("api/orders")]
    public class OrderController : Controller
    {
        private readonly IOrdersService _orderService;
        private readonly IUsersService _usersService;


        public OrderController(IOrdersService orderService, IUsersService usersService)
        {
            _orderService = orderService;
            _usersService = usersService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            // Get Orders from current User
            long userId = Convert.ToInt64(_usersService.GetCurrentUserId());
            Tuple<int, List<Order>> orders = await _orderService.FetchPageFromUser(userId);

            return StatusCodeAndDtoWrapper.BuildSuccess(OrdersListDtoResponse.Build(orders.Item2, Request.Path,
                currentPage: page, pageSize: pageSize, totalItemCount: orders.Item1));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdersById(long id)
        {
            var order = await _orderService.FetchById(id, includeOrderItems: true, includeAddress: true);
            if (order == null)
                return StatusCodeAndDtoWrapper.BuildGeneric(new ErrorDtoResponse("Not Found"), statusCode: 404);

            //return NotFound();

            return new StatusCodeAndDtoWrapper(OrderDetailsDto.Build(order, false));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto form)
        {
            var order = await _orderService.Create(form, await _usersService.GetCurrentUserAsync());
            if (order != null)
            {
                return StatusCodeAndDtoWrapper.BuildGeneric(OrderDetailsDto.Build(order));
            }
            else
            {
                return StatusCodeAndDtoWrapper.BuildErrorResponse("Something went wrong");
            }
        }
    }
}
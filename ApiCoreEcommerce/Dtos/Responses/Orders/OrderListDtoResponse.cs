using System.Collections.Generic;
using ApiCoreEcommerce.Dtos.Responses.Products;
using ApiCoreEcommerce.Dtos.Responses.Shared;
using ApiCoreEcommerce.Entities;
using BlogDotNet.Models;

namespace ApiCoreEcommerce.Dtos.Responses.Orders
{
    public class OrdersListDtoResponse : PagedDto
    {
        public IEnumerable<OrderDto> Orders { get; set; }

        public static OrdersListDtoResponse Build(List<Order> orders,
            string basePath,
            int currentPage, int pageSize, int totalItemCount)
        {
            List<OrderDto> orderDtos = new List<OrderDto>(orders.Count);
            foreach (var order in orders)
            {
                orderDtos.Add(OrderDto.Build(order));
            }

            return new OrdersListDtoResponse
            {
                PageMeta = new PageMeta(orders.Count, basePath, currentPageNumber: currentPage, requestedPageSize: pageSize,
                    totalItemCount: totalItemCount),
                Orders = orderDtos
            };
        }
    }
}
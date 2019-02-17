using System.Collections;
using System.Collections.Generic;
using ApiCoreEcommerce.Dtos.Responses.OrderItems;
using ApiCoreEcommerce.Dtos.Responses.Products;
using ApiCoreEcommerce.Dtos.Responses.Shared;
using ApiCoreEcommerce.Dtos.Responses.User;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Enums;

namespace ApiCoreEcommerce.Dtos.Responses.Orders
{
    public class OrderDetailsDto : SuccessResponse
    {
        public long Id { get; set; }
        public string TrackingNumber { get; set; }
        public ShippingStatus OrderStatus { get; set; }
        public UserBasicEmbeddedInfoDto User { get; set; }
        public AddressDto Address { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; }

        public static OrderDetailsDto Build(Order order, bool includeUser = false)
        {
            List<OrderItemDto> orderItemDtos = new List<OrderItemDto>(order.OrderItems.Count);
                
            foreach (var orderItem in order.OrderItems)
            {
                orderItemDtos.Add(OrderItemDto.Build(orderItem));
            }
            
            var dto = new OrderDetailsDto
            {
                Id = order.Id,
                TrackingNumber = order.TrackingNumber,
                OrderStatus = order.OrderStatus,
                Address = AddressDto.Build(order.Address),
                OrderItems = orderItemDtos
            };

            return dto;
        }
    }
}
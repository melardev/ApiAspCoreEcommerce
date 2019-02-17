using System;
using System.Collections.Generic;
using ApiCoreEcommerce.Dtos.Responses.Category;
using ApiCoreEcommerce.Dtos.Responses.Products;
using ApiCoreEcommerce.Dtos.Responses.Tag;
using ApiCoreEcommerce.Dtos.Responses.User;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Enums;

namespace ApiCoreEcommerce.Dtos.Responses.Orders
{
    public class OrderDto
    {
        public long Id { get; set; }
        public string TrackingNumber { get; set; }
        public ShippingStatus OrderStatus { get; set; }
        public UserBasicEmbeddedInfoDto User { get; set; }
        public AddressDto Address { get; set; }


        public static OrderDto Build(Order order, bool includeUser = false)
        {
            var dto = new OrderDto
            {
                Id = order.Id,
                TrackingNumber = order.TrackingNumber,
                OrderStatus = order.OrderStatus,
                Address = AddressDto.Build(order.Address),
            };

            if (includeUser)
                dto.User = UserBasicEmbeddedInfoDto.Build(order.User);

            return dto;
        }
    }
}
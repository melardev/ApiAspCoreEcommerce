using ApiCoreEcommerce.Entities;

namespace ApiCoreEcommerce.Dtos.Responses.OrderItems
{
    public class OrderItemDto
    {
        public static OrderItemDto Build(OrderItem orderItem)
        {
            return new OrderItemDto
            {
                Name = orderItem.Name,
                Price = orderItem.Price,
                Slug = orderItem.Slug
            };
        }

        public string Slug { get; set; }

        public int Price { get; set; }

        public string Name { get; set; }
    }
}
using System;
using System.Collections.ObjectModel;

namespace ApiCoreEcommerce.Dtos.Requests.Orders
{
    public class CreateOrderDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }


        private string CardNumber { get; set; }

        public string Address { get; set; }

        public long? AddressId { get; set; }


        public string City { get; set; }


        public string Country { get; set; }

        public string PhoneNumber { get; set; }

        public String ZipCode { get; set; }

        public Collection<CartItemDto> CartItems { get; set; }
    }

    public class CartItemDto
    {
        public long Id { get; set; }
        public int Quantity { get; set; }
    }
}
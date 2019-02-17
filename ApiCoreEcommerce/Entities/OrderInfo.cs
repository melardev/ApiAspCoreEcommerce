using ApiCoreEcommerce.Enums;

namespace ApiCoreEcommerce.Entities
{
    public class OrderInfo
    {
        public long Id { get; set; }
        public Address Address { get; set; }
        public string TrackingNumber { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
    }
}
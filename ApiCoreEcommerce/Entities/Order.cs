using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiCoreEcommerce.Enums;
using ApiCoreEcommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ApiCoreEcommerce.Entities
{
    public class Order
    {
        public long Id { get; set; }
        [BindNever] public ICollection<OrderItem> OrderItems { get; set; }

        [Required(ErrorMessage = "Please enter the address to ship to")]

        public ApplicationUser User { get; set; }

        public long? UserId { get; set; }
        // public OrderInfo OrderInfo { get; set; }

        public Address Address { get; set; }
        public long AddressId { get; set; }
        public string TrackingNumber { get; set; }
        public ShippingStatus OrderStatus { get; set; }
        [NotMapped] public decimal Sum { get; set; }

        public DateTime CreatedAt { get; set; }
        private DateTime UpdatedAt { get; set; }
    }
}
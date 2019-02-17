using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace ApiCoreEcommerce.Entities
{
    public class Product
    {
        public Product()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
          
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public string Slug { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
        public ICollection<ProductTag> ProductTags { get; set; }
        public ICollection<Rating> Ratings { get; set; }

        [NotMapped] public int CommentsCount { get; set; }
        
        public ICollection<ProductImage> ProductImages { get; set; }
        
        public DateTime PublishAt { get; set; }
        public Collection<Comment> Comments { get; set; }
        
        
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
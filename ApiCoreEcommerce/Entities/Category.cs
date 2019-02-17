using System;
using System.Collections.Generic;

namespace ApiCoreEcommerce.Entities
{
    public class Category
    {
        public Category()
        {
            ProductCategories = new HashSet<ProductCategory>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt;
        public DateTime UpdatedAt;

        public ICollection<ProductCategory> ProductCategories { get; set; }
        
        public ICollection<CategoryImage> CategoryImages { get; set; }
        
    }
}
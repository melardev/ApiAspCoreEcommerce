using System;
using System.Collections.Generic;
using System.Linq;
using ApiCoreEcommerce.Dtos.Responses.Category;
using ApiCoreEcommerce.Dtos.Responses.Tag;
using ApiCoreEcommerce.Entities;

namespace ApiCoreEcommerce.Dtos.Responses.Products
{
    public class ProductSummaryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }

        public int CommentsCount { get; set; }

        public List<string> Categories { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<string> ImageUrls { get; set; }

        public DateTime PublishAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static ProductSummaryDto Build(Product product)
        {
            return new ProductSummaryDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Price = product.Price,
                Stock = product.Stock,
                CommentsCount = product.CommentsCount,
                Categories = CategoryOnlyNameDto.BuildAsStringList(product.ProductCategories),
                Tags = TagOnlyNameDto.BuildAsStringList(product.ProductTags),
                ImageUrls = product.ProductImages.Select(pi => pi.FilePath),
                PublishAt = product.PublishAt,
            };
        }
    }
}
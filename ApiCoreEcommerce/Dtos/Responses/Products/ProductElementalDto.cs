using ApiCoreEcommerce.Entities;

namespace ApiCoreEcommerce.Dtos.Responses.Products
{
    public class ProductElementalDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        

        public static ProductElementalDto Build(Product product)
        {
            return new ProductElementalDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
            };
        }
    }
}
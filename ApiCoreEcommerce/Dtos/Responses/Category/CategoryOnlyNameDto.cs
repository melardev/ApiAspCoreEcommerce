using System.Collections.Generic;
using ApiCoreEcommerce.Entities;

namespace ApiCoreEcommerce.Dtos.Responses.Category
{
    public class CategoryOnlyNameDto
    {
        public string Name { get; set; }

        public static List<string> BuildAsStringList(ICollection<ProductCategory> productCategories)
        {
            if (productCategories == null)
                return null;
            List<string> result = new List<string>(productCategories.Count);
            foreach (var productCategory in productCategories)
            {
                result.Add(productCategory.Category?.Name);
            }

            return result;
        }
    }
}
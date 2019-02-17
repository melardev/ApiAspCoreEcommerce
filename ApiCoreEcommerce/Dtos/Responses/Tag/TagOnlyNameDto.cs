using System.Collections.Generic;
using ApiCoreEcommerce.Entities;

namespace ApiCoreEcommerce.Dtos.Responses.Tag
{
    public class TagOnlyNameDto
    {
        public string Name { get; set; }

        public static List<string> BuildAsStringList(IEnumerable<ProductTag> productTags)
        {
            if (productTags == null)
                return null;
            //List<string> result = new List<string>(productTags.Count);
            List<string> result = new List<string>(20);
            foreach (var productTag in productTags)
            {
                result.Add(productTag?.Tag?.Name);
            }

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiCoreEcommerce.Entities;

namespace ApiCoreEcommerce.Dtos.Responses
{
    public class ProductsListDto
    {
        public static string Build(Task<Tuple<int,List<Product>>> products)
        {
            throw new NotImplementedException();
        }
    }
}
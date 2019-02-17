using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCoreEcommerce.Entities;

namespace ApiCoreEcommerce.Services.Interfaces
{
    public interface IAddressesService
    {
        Task<Tuple<int, List<Address>>> FetchPage(int page = 1, int pageSize = 5);
        Task<Tuple<int, List<Address>>> FetchPageByUser(ApplicationUser user, int page, int pageSize);

       

        Task<Address> FetchByIdAsync(long? id);

        Task<Address> Create(ApplicationUser applicationUser, string dtoFirstName, string dtoLastname,
            string dtoCountry, string dtoCity, string dtoStreetAddress, string dtoZipCode);
    }
}
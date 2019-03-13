using System.Collections.Generic;
using ApiCoreEcommerce.Dtos.Responses.Shared;
using ApiCoreEcommerce.Entities;
using BlogDotNet.Models;

namespace ApiCoreEcommerce.Dtos.Responses.Products
{
    public class AddressesListDtoResponse : PagedDto
    {
        public IEnumerable<AddressDto> Addresses { get; set; }
//    public int SortBy {get; set;}


        public static AddressesListDtoResponse Build(List<Address> addresses, string basePath,
            int currentPage, int pageSize, int totalItemCount)
        {
            List<AddressDto> addressDtos = new List<AddressDto>(addresses.Count);
            
            foreach (var address in addresses)
                addressDtos.Add(AddressDto.Build(address));


            return new AddressesListDtoResponse
            {
                PageMeta = new PageMeta(addresses.Count, basePath, currentPageNumber: currentPage, requestedPageSize: pageSize,
                    totalItemCount: totalItemCount),
                Addresses = addressDtos
            };
        }
    }
}
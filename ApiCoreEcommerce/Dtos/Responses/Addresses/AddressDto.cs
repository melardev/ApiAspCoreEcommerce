using ApiCoreEcommerce.Dtos.Responses.Shared;
using ApiCoreEcommerce.Dtos.Responses.User;
using ApiCoreEcommerce.Entities;

namespace ApiCoreEcommerce.Dtos.Responses.Products
{
    public class AddressDto : SuccessResponse
    {
        public static AddressDto Build(Address address, bool includeUser = false)
        {
            var dto = new AddressDto
            {
                Id = address.Id,
                City = address.City,
                Country = address.Country,
                ZipCode = address.ZipCode,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Address = address.StreetAddress
            };

            if (includeUser)
                dto.User = UserBasicEmbeddedInfoDto.Build(address.User);
            return dto;
        }

        public UserBasicEmbeddedInfoDto User { get; set; }

        public long Id { get; set; }
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Address { get; set; }
    }
}
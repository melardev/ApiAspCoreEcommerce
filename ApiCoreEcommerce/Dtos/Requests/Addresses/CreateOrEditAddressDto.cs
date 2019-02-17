namespace ProductManager.Models.AccountViewModels.Requests.Addresses
{
    public class CreateOrEditAddressDto
    {
     
            public string Lastname { get; set; }
            public string Country { get; set; }
            public string FirstName { get; set; }
            public string City { get; set; }
            public string StreetAddress { get; set; }
            public string ZipCode { get; set; }
        
    }
}
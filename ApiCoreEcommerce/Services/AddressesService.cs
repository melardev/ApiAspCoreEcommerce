using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCoreEcommerce.Data;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiCoreEcommerce.Services
{
    public class AddressesService : IAddressesService
    {
        private readonly ApplicationDbContext _context;

        public AddressesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tuple<int, List<Address>>> FetchPage(int page = 1, int pageSize = 5)
        {
            var queryable = _context.Addresses;
            return await FetchPageFromQueryable(queryable, page, pageSize);
        }

        public async Task<Tuple<int, List<Address>>> FetchPageByUser(ApplicationUser user, int page, int pageSize)
        {
            var count = _context.Addresses.Count(a => a.User.Id == user.Id);
            var queryable = _context.Addresses.Where(a => a.User == user)
                .Include(a => a.User);
            return await FetchPageFromQueryable(queryable, page, pageSize);
        }

        private async Task<Tuple<int, List<Address>>> FetchPageFromQueryable(IQueryable<Address> queryable, int page,
            int pageSize)
        {
            var count = await queryable.CountAsync();
            var addresses = await queryable.Skip((page - 1) * pageSize).Take(pageSize)
                .Include(a => a.User)
                .ToListAsync();

            return await Task.FromResult(Tuple.Create(count, addresses));
        }

        public async Task<Address> FetchByIdAsync(long? id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        public async Task<Address> Create(ApplicationUser applicationUser, string dtoFirstName, string dtoLastname,
            string dtoCountry, string dtoCity, string dtoStreetAddress, string dtoZipCode)
        {
            Address address = new Address
            {
                FirstName = dtoFirstName,
                LastName = dtoLastname,
                User = applicationUser,
                Country = dtoCountry,
                City = dtoCity,
                StreetAddress = dtoStreetAddress,
                ZipCode = dtoZipCode
            };

            _context.Addresses.Add(address);

            await _context.SaveChangesAsync();
            return address;
        }

    }
}
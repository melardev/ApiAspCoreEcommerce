using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiCoreEcommerce.Entities;
using Microsoft.AspNetCore.Http;

namespace ApiCoreEcommerce.Services.Interfaces
{
    public interface ICategoriesService
    {
        int Count();
        void Delete(int id);
        Category FetchById(int id);
        void Create(Category cat);
        void Update(Category cat);
        Task<Tuple<int, List<Category>>> FetchPageWithImages(int page, int pageSize);

        Task<Category> Create(string name, string description, List<IFormFile> files,
            long? userId = null);

        Task<Tuple<int, List<Category>>> FetchPage(int page, int pageSize);
    }
}
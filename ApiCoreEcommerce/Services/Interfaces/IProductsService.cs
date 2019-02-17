using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiCoreEcommerce.Dtos.Responses.Category;
using ApiCoreEcommerce.Dtos.Responses.Products;
using ApiCoreEcommerce.Dtos.Responses.Tag;
using ApiCoreEcommerce.Entities;
using Microsoft.AspNetCore.Http;

namespace ApiCoreEcommerce.Services.Interfaces
{
    public interface IProductsService
    {
        Task<Tuple<int, List<Product>>> FetchPage(int page = 1, int pageSize = 5);
        Task Create(Product product);

        Task<Task> Update(Product product);

        Task<int> Delete(long id);

        Task<int> Delete(Product product);

        Task<Product> FetchById(long id, bool onlyIfPublished = false);
        Task<List<Product>> FetchAll();

        Product FetchByIdSync(long id);

        Task<Product> FetchBySlug(string slug);
        Task<Tuple<int, List<Product>>> FetchBySearchTerm(string term, int page, int pageSize);
        Task<List<Product>> FetchByIdInRetrieveNamePriceAndSlug(IEnumerable<long> productIds);
        Task<Product> GetProductBySlug(string slug);
        Task<Tuple<int, List<Product>>> FetchPageByCategory(string category, int pageSize, int page);
        Task<long> FetchBoughtManyTimes(Product product);
        Task SaveProduct(Product product);

        Task<Product> Create(string name, string description,
            IEnumerable<TagOnlyNameDto> tagOnlyNameDtos, IEnumerable<CategoryOnlyNameDto> categoryOnlyNameDtos,
            List<IFormFile> images);

        Task<Product> Create(string name, string description, int price, int stock, List<Tag> tags,
            List<Category> categories, List<IFormFile> images, bool processTags = true, bool processCategories = true);

        Task<Product> Update(string slug, CreateOrEditProduct dto);
        Product DeleteProductById(long id);
        Task<int> Delete(string slug);
    }
}
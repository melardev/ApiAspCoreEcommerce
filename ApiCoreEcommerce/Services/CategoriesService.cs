using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCoreEcommerce.Data;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiCoreEcommerce.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;

        public CategoriesService(ApplicationDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public int Count()
        {
            return _context.Categories.Count();
        }

        public async Task<Tuple<int, List<Category>>> FetchPage(int page, int pageSize)
        {
            var queryable = _context.Categories;
            var count = await queryable.CountAsync();
            var results = await queryable.Include(t => t.CategoryImages).Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            return await Task.FromResult(Tuple.Create(count, results));
        }

        public async Task<Tuple<int, List<Category>>> FetchPageWithImages(int page, int pageSize)
        {
            var queryable = _context.Categories.Include(c => c.CategoryImages)
                .Where(t => t.CategoryImages != null && t.CategoryImages.Count > 0);
            var count = await queryable.CountAsync();
            var results = await queryable.Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            return await Task.FromResult(Tuple.Create(count, results));
        }

        public Category FetchById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }


        public async Task<Category> Create(string name, string description, List<IFormFile> files,
            long? userId = null)
        {
            ICollection<CategoryImage> fileUploads = new List<CategoryImage>(files.Count);
            foreach (IFormFile file in files)
            {
                FileUpload fileUpload = await _storageService.UploadFormFile(file, "categories");

                fileUploads.Add(new CategoryImage
                {
                    OriginalFileName = fileUpload.OriginalFileName,
                    FileName = fileUpload.FileName,
                    FilePath = fileUpload.FilePath,
                    FileSize = file.Length
                });
            }


            var category = new Category
            {
                Name = name,
                Description = description,
                CategoryImages = fileUploads
            };
            _context.Categories.Add(category);

            await _context.SaveChangesAsync();
            return category;
        }


        public void Create(Category cat)
        {
            _context.Categories.Add(cat);
        }


        public void Update(Category cat)
        {
            _context.Categories.Update(cat);
        }

        public void Delete(int id)
        {
            var category = FetchById(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
        }
    }
}
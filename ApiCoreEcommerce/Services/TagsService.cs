using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiCoreEcommerce.Data;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiCoreEcommerce.Services
{
    public class TagsService : ITagsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;


        public TagsService(ApplicationDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<Tuple<int, List<Tag>>> FetchPage(int page, int pageSize)
        {
            var queryable = _context.Tags;
            var count = await queryable.CountAsync();
            var results = await queryable.Include(t => t.TagImages).Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            return await Task.FromResult(Tuple.Create(count, results));
        }

        public async Task<Tuple<int, List<Tag>>> FetchPageWithImages(int page, int pageSize)
        {
            var queryable = _context.Tags.Include(t => t.TagImages)
                .Where(t => t.TagImages != null && t.TagImages.Count > 0);
            var count = await queryable.CountAsync();
            var results = await queryable.Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            return await Task.FromResult(Tuple.Create(count, results));
        }

        public async Task<Tag> Create(string name, string description, List<IFormFile> files)
        {
            ICollection<TagImage> fileUploads = new List<TagImage>(files.Count);
            foreach (IFormFile file in files)
            {
                FileUpload fileUpload = await _storageService.UploadFormFile(file, "tags");

                fileUploads.Add(new TagImage
                {
                    OriginalFileName = fileUpload.OriginalFileName,
                    FileName = fileUpload.FileName,
                    FilePath = fileUpload.FilePath,
                    FileSize = file.Length,
                });
            }


            var tag = new Tag
            {
                Name = name,
                Description = description,
                TagImages = fileUploads
            };
            _context.Tags.Add(tag);

            await _context.SaveChangesAsync();
            return tag;
        }
    }
}
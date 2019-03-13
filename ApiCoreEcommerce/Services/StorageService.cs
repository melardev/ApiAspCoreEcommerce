using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Services.Interfaces;
using BlogDotNet.Errors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApiCoreEcommerce.Services
{
    public class StorageService : IStorageService
    {
        // readonly string _separator = Path.DirectorySeparatorChar.ToString();
        readonly string separator = "/";

        IHttpContextAccessor _httpContext;

        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public StorageService(IHttpContextAccessor httpContext, ILogger<StorageService> logger,
            IHostingEnvironment hostingEnvironment)
        {
            _httpContext = httpContext;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            if (!Directory.Exists(ImageUploadDirectory))
                CreateFolder("");
        }

        
        public string WebRoot => _hostingEnvironment.WebRootPath.Replace("\\", "/");

        public string ContentRootPath => _hostingEnvironment.ContentRootPath.Replace("\\", "/");

        public string ImageUploadDirectory
        {
            get
            {
                var path = WebRoot ??
                           Path.Combine(ContentRootPath, "wwwroot");

                path = Path.Combine(path, "images");

                return path;
            }
        }

        public async Task<FileUpload> UploadFormFile(IFormFile file, string path = "")
        {
            VerifyPath(path);
            // System.IO.Path.GetExtension(file.FileName);
            var fileName = GetRandomFileName() + GetFileExtension(file.FileName);
            var filePath = string.IsNullOrEmpty(path)
                ? Path.Combine(ImageUploadDirectory, fileName)
                : Path.Combine(ImageUploadDirectory, path + separator + fileName);

            filePath = filePath.Replace("\\", "/");

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);

                return new FileUpload
                {
                    OriginalFileName = file.FileName,
                    FileName = fileName,
                    FilePath = TrimFilePath(filePath),
                    FileSize = file.Length,
                };
            }
        }

        public string GetFileExtension(string fileName)
        {
            string[] parts = fileName.Split(".");
            string extension = parts[parts.Length - 1];
            if (extension.StartsWith("png", StringComparison.OrdinalIgnoreCase)
                || extension.StartsWith("jpeg", StringComparison.OrdinalIgnoreCase)
                || extension.StartsWith("jpg",
                    StringComparison.OrdinalIgnoreCase))
                return "." + extension;
            else
            {
                throw new PermissionDeniedException(
                    "For security reasons it is not allowed to upload files other than png or jpeg");
            }
        }

        public void CreateFolder(string path)
        {
            var dir = GetFullPath(path);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public void VerifyPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var dir = Path.Combine(ImageUploadDirectory, path);

                if (!Directory.Exists(dir))
                {
                    CreateFolder(dir);
                }
            }
        }

        public string TrimFilePath(string path)
        {
            var p = path.Replace(WebRoot, "");
            if (p.StartsWith("\\")) p = p.Substring(1);
            return p;
        }

        public string GetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return ImageUploadDirectory;
            else
                return Path.Combine(ImageUploadDirectory, path.Replace("/", separator));
        }

        public string GetRandomFileName()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", "");
            return path;
        }
    }
}
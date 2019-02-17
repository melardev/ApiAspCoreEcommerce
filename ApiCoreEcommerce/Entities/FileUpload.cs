using System;

namespace ApiCoreEcommerce.Entities
{
    public class FileUpload
    {
        public long Id { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }

        public long FileSize { get; set; }

       
        public bool isFeaturedImage { get; set; }
    }
}
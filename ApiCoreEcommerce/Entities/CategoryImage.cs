using System.Collections.Generic;

namespace ApiCoreEcommerce.Entities
{
    public class CategoryImage : FileUpload
    {
        public Category Category { get; set; }
        public long CategoryId { get; set; }
        
    }
}
using System.Collections.Generic;

namespace ApiCoreEcommerce.Entities
{
    public class TagImage : FileUpload
    {
        public Tag Tag { get; set; }
        public long TagId { get; set; }
    }
}
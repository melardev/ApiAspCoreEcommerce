namespace ApiCoreEcommerce.Entities
{
    public class ProductImage : FileUpload
    {
        // Uploader
        public Product Product { get; set; }
        public long ProductId { get; set; }
    }
}
using BlogDotNet.Models;

namespace ApiCoreEcommerce.Dtos.Responses.Shared
{
    public abstract class PagedDto : SuccessResponse
    {
        public PageMeta PageMeta { get; set; }
    }
}
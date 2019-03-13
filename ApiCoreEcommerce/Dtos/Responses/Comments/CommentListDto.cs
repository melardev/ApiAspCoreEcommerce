using System.Collections.Generic;
using ApiCoreEcommerce.Dtos.Responses.Products;
using ApiCoreEcommerce.Dtos.Responses.Shared;
using BlogDotNet.Models;
using BlogDotNet.Models.ViewModels.User;

namespace ApiCoreEcommerce.Dtos.Responses.Comments
{
    class CommentListDto : SuccessResponse
    {
        
        public PageMeta PageMeta { get; set; }
        public ICollection<CommentDetailsDto> Comments { get; set; }

        public static CommentListDto Build(ICollection<Entities.Comment> comments,
            string basePath,
            int currentPage, int pageSize, int totalItemCount)
        {
            ICollection<CommentDetailsDto> result = new List<CommentDetailsDto>();

            foreach (var comment in comments)
                result.Add(CommentDetailsDto.Build(comment, false, true));

            return new CommentListDto
            {
                Success = true,
                PageMeta = new PageMeta(result.Count, basePath, currentPageNumber: currentPage, requestedPageSize: pageSize,
                    totalItemCount: totalItemCount),
                Comments = result
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiCoreEcommerce.Entities;
using BlogDotNet.Models.ViewModels.Requests.Comment;

namespace ApiCoreEcommerce.Services.Interfaces
{
    public interface ICommentsService
    {
        Task<Comment> CreateAsync(ApplicationUser user, string productSlug, CreateOrEditCommentDto dto,
            long userId);

        Task<Comment> FetchCommentByIdAsync(long id, bool includeUser = false);
        Task<int> DeleteAsync(long id);
        Task<Tuple<int, List<Comment>>> FetchPageByProduct(string slug, int page = 1, int pageSize = 5);
        Task<int> UpdateAsync(Comment comment, CreateOrEditCommentDto dto);
    }
}
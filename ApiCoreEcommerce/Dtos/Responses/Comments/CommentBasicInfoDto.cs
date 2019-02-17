using System;
using ApiCoreEcommerce.Dtos.Responses.User;
using ApiCoreEcommerce.Entities;
using BlogDotNet.Models.ViewModels.User;

namespace ApiCoreEcommerce.Dtos.Responses.Comments
{
    public class CommentIntrinsicInfoDto
    {
        public string Content { get; set; }
        public UserBasicEmbeddedInfoDto User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //public Comment Comment {get; set;}
        public static CommentIntrinsicInfoDto Build(Comment comment)
        {
            return new CommentIntrinsicInfoDto
            {
                Content = comment.Content,
                User = UserBasicEmbeddedInfoDto.Build(comment.User),
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }
    }
}
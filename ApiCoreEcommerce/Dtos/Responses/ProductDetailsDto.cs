using System;
using System.Collections.Generic;
using System.Linq;
using ApiCoreEcommerce.Dtos.Responses.Category;
using ApiCoreEcommerce.Dtos.Responses.Comments;
using ApiCoreEcommerce.Dtos.Responses.Shared;
using ApiCoreEcommerce.Dtos.Responses.Tag;
using ApiCoreEcommerce.Entities;

namespace ApiCoreEcommerce.Dtos.Responses
{
    public class ProductDetailsDto : SuccessResponse
    {
        public long Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime ModifiedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public List<string> Tags { get; set; }

        public IEnumerable<string> ImageUrls { get; set; }

        // public IEnumerable<CommentIntrinsicInfoDto> Comments { get; set; }
        public IEnumerable<CommentDetailsDto> Comments { get; set; }

        public static ProductDetailsDto Build(Product product)
        {
            var commentDtos = new List<CommentDetailsDto>();
            if (product.Comments != null)
            {
                foreach (var comment in product.Comments)
                {
                    commentDtos.Add(CommentDetailsDto.Build(comment));
                }
            }

            return new ProductDetailsDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                PublishedAt = product.PublishAt,
                Comments = commentDtos,
                Tags = TagOnlyNameDto.BuildAsStringList(product.ProductTags),
                Categories = CategoryOnlyNameDto.BuildAsStringList(product.ProductCategories),
                ImageUrls = product.ProductImages.Select(pi => pi.FilePath)
            };
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using ApiCoreEcommerce.Dtos.Responses.Shared;

namespace ApiCoreEcommerce.Dtos.Responses.Tag
{
    public class TagDto : SuccessResponse
    {
        public long Id { get; set; }
        public List<string> ImageUrls { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public static TagDto Build(Entities.Tag tag)
        {
            List<string> imageUrls = new List<string>();
            if (tag.TagImages != null)
            {
                foreach (var tagImage in tag.TagImages)
                {
                    imageUrls.Add(tagImage.FilePath);
                }
            }

            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description,
                ImageUrls = imageUrls
            };
        }
    }
}
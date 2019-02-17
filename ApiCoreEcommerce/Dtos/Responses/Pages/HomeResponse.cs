using System.Collections.Generic;
using ApiCoreEcommerce.Dtos.Responses.Category;
using ApiCoreEcommerce.Dtos.Responses.Shared;
using ApiCoreEcommerce.Dtos.Responses.Tag;

namespace ApiCoreEcommerce.Dtos.Responses.Pages
{
    public class HomeResponse : SuccessResponse
    {
        public static HomeResponse Build(List<Entities.Tag> tags, List<Entities.Category> categories)
        {
            List<TagDto> tagDtos = new List<TagDto>(tags.Count);
            List<CategoryDto> categoryDtos = new List<CategoryDto>(tags.Count);
            foreach (var tag in tags)
            {
                tagDtos.Add(TagDto.Build(tag));
            }

            foreach (var category in categories)
            {
                categoryDtos.Add(CategoryDto.Build(category));
            }

            return new HomeResponse
            {
                Tags = tagDtos,
                Categories = categoryDtos
            };
        }

        public List<CategoryDto> Categories { get; set; }

        public List<TagDto> Tags { get; set; }
    }
}
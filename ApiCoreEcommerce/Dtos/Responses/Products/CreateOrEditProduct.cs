using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApiCoreEcommerce.Dtos.Responses.Category;
using ApiCoreEcommerce.Dtos.Responses.Tag;
using Newtonsoft.Json;

namespace ApiCoreEcommerce.Dtos.Responses.Products
{
    public class CreateOrEditProduct
    {
   
        [JsonProperty(PropertyName = "name")]
        [Required]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "body")]
        [Required]
        public string Body { get; set; }

        [JsonProperty(propertyName: "description")]
        [Required]
        public string Description { get; set; }

        // [Required]
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime PublishedOn { get; set; }

       
        [JsonProperty(PropertyName = "categories")]
        public IEnumerable<CategoryOnlyNameDto> Categories { get; set; }

        [JsonProperty(PropertyName = "tags")] public IEnumerable<TagOnlyNameDto> Tags { get; set; }
        
        public DateTime PublishAt { get; set; }
    }
}
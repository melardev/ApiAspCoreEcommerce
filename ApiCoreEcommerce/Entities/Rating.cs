using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCoreEcommerce.Entities
{
    [Table("Ratings")]
    public class Rating
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }

        // [ForeignKey("Product")]
        public long ProductId { get; set; }

        [Display(Name = "Rating")] // In dotnet we can not name Fields same as Scoring ,so fix it with annotations
        public long Value { get; set; }

        public long UserId { get; set; }
        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
        public Comment Comment { get; set; }
        public long CommentId { get; set; }
    }
}
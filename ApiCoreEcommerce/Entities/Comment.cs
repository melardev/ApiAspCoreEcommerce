using System;
using System.Collections.Generic;

namespace ApiCoreEcommerce.Entities
{
    public class Comment
    {
        public Comment()
        {
            //CreatedAt = DateTime.Now;
        }

        public long Id { get; set; }
        public string Content { get; set; }
        public int? Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Product Product { get; set; }
        public long ProductId { get; set; }


        public virtual HashSet<Comment> Replies { get; set; }
        public ApplicationUser User { get; set; }
        public long UserId { get; set; }

        // public Rating Rating { get; set; }
        public long RatingId { get; set; }

        public string RenderContent()
        {
            throw new NotImplementedException();
        }
    }
}
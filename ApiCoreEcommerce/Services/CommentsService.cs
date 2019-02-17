using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApiCoreEcommerce.Data;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Services.Interfaces;
using BlogDotNet.Models.ViewModels.Requests.Comment;
using Microsoft.EntityFrameworkCore;

namespace ApiCoreEcommerce.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductsService _productsService;
        private readonly HtmlEncoder _htmlEncoder;

        public CommentsService(ApplicationDbContext context, IProductsService productService, HtmlEncoder htmlEncoder)
        {
            _context = context;
            _productsService = productService;
            _htmlEncoder = htmlEncoder;
        }

       
        public async Task<Tuple<int, List<Comment>>> FetchPageByProduct(string slug, int page = 1, int pageSize = 5)
        {
            IQueryable<Comment> queryable = _context.Comments.Where(c => c.Product.Slug.Equals(slug))
                .Include(c => c.Product)
                .Include(c => c.User)
                .Select(c => new Comment
                {
                    Id = c.Id,
                    Rating = c.Rating,
                    Content = c.Content,

                    ProductId = c.ProductId,
                    Product = new Product
                    {
                        Id = c.ProductId,
                        Name = c.Product.Name,
                        Slug = c.Product.Slug
                    },
                    User = new ApplicationUser
                    {
                        Id = c.UserId,
                        UserName = c.User.UserName
                    },
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                });

            var count = await queryable.CountAsync();


            var comments = await queryable.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Tuple.Create(count, comments);
        }
        
        public async Task<Comment> FetchCommentByIdAsync(long id, bool includeUser = false)
        {
            if (includeUser)
                return await _context.Comments.Include(c => c.User).FirstAsync(c => c.Id == id);
            else
                return await _context.Comments.FindAsync(id);
        }

        public async Task<Comment> CreateAsync(ApplicationUser user, string productSlug, CreateOrEditCommentDto dto,
            long userId)
        {
            var product = await _productsService.FetchBySlug(productSlug);

            var comment = new Comment()
            {
                Product = product,
                ProductId = product.Id,
                Content = _htmlEncoder.Encode(dto.Content),
                Rating = dto.Rating,
                UserId = userId,
                User = user,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };


            await _context.Comments.AddAsync(comment);
            product.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<int> UpdateAsync(Comment comment, CreateOrEditCommentDto dto)
        {
            comment.Content = _htmlEncoder.Encode(dto.Content);
            comment.Rating = dto.Rating;
            return await _context.SaveChangesAsync();
        }
        
        public async Task<int> DeleteAsync(long id)
        {
            Comment comment = await _context.Comments.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (comment == null)
            {
                return -1;
            }

            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync();
        }

        
    }
}
using System;
using System.Threading.Tasks;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ApiCoreEcommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext
        <
            ApplicationUser,
            ApplicationRole,
            long,
            IdentityUserClaim<long>,
            AppUserRole,
            IdentityUserLogin<long>,
            IdentityRoleClaim<long>,
            IdentityUserToken<long>>,
        IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder =
                new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlite("DataSource=app.db");

            var appDbContext = new ApplicationDbContext(optionsBuilder.Options);
            return appDbContext;
        }

// In database store only Orders and Products, and Users(managed by Identity)
// Cart and CartItems are stored in session, 
        public DbSet<Product> Products { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        public DbSet<TagImage> TagImages { get; set; }
        public DbSet<CategoryImage> CategoryImages { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            /* Not working in EF Core
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Following)
                .WithMany(u => u.Followers)
                .Map(x => x.MapLeftKey("UserId").MapRightKey("FollowerId").ToTable("UserRelations"));
            */

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Slug).IsRequired();
                entity.HasIndex(p => p.Slug).IsUnique(true);
            });

            // modelBuilder.Entity<Product>().HasIndex(p => p.Slug).IsUnique(true);


            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Description);

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => new {e.CategoryId, e.ProductId});

                entity.Property(e => e.CategoryId);

                entity.Property(e => e.ProductId);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                // entity.Property(e => e.ProductId);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserId);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.User).WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId).IsRequired(false)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(o => o.OrderId).IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(o => o.Address).WithMany((string) null)
                    .HasForeignKey(o => o.AddressId).IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(oi => oi.User)
                    .WithMany((string) null).HasForeignKey(oi => oi.UserId)
                    .IsRequired(false).OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId).IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            /*
             I am not using it for the moment I really have my doubts on how to manage Rating/Comment/Replies feature.
            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(l => new {l.UserId, l.ProductId});

                // entity.Property(e => e.ProductId);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserId);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull);

                // One Rating to 0 or 1 Comment.
                entity.HasOne<Comment>(r => r.Comment) // A Rating may not have a Comment (One Rating to 0 or 1 Comment)
                    .WithOne(c => c.Rating) // All comments have a Rating
                    .HasForeignKey<Rating>(r => r.CommentId);
            });
*/

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Description);

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<ProductTag>(entity =>
            {
                entity.HasKey(e => new {e.TagId, e.ProductId});

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductTags)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ProductTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });


            modelBuilder.Entity<TagImage>(entity =>
            {
                entity.HasBaseType<FileUpload>();
                entity.HasOne(ti => ti.Tag)
                    .WithMany(t => t.TagImages)
                    .HasForeignKey(ti => ti.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CategoryImage>(entity =>
            {
                entity.HasBaseType<FileUpload>();
                entity.HasOne(ti => ti.Category)
                    .WithMany(t => t.CategoryImages)
                    .HasForeignKey(ti => ti.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasBaseType<FileUpload>();
                entity.HasOne(pi => pi.Product)
                    .WithMany(t => t.ProductImages)
                    .HasForeignKey(ti => ti.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public static async Task<Task> Seed(IServiceProvider services, IConfigurationRoot config)
        {
            await Seeder.Seed(services);
            return Task.CompletedTask;
        }
    }
}
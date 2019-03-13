using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiCoreEcommerce.Data;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Enums;
using ApiCoreEcommerce.Errors;
using ApiCoreEcommerce.Infrastructure.Extensions;
using ApiCoreEcommerce.Services.Interfaces;
using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Address = ApiCoreEcommerce.Entities.Address;

namespace ApiCoreEcommerce.Seeds
{
    public class Seeder
    {
        private static async Task SeedAuthenticatedUsersAndRole(IServiceProvider services)
        {
            Faker faker = new Faker();
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService settingsService = services.GetService<IConfigurationService>();

                UserManager<ApplicationUser> userManager =
                    serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                RoleManager<ApplicationRole> roleManager =
                    serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();

                string standardUserRoleName = settingsService.GetStandardUserRoleName();
//                if (await roleManager.FindByNameAsync(roleAuthor) == null)
                IdentityResult result = IdentityResult.Success;
                if (!(await roleManager.RoleExistsAsync(standardUserRoleName)))
                {
                    result = await roleManager.CreateAsync(new ApplicationRole(standardUserRoleName));
                    if (!result.Succeeded)
                    {
                        throw new UnexpectedApplicationStateException();
                    }
                }

                if (result.Succeeded)
                {
                    ApplicationDbContext applicationDbContext =
                        serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var usersCount = applicationDbContext.Users.Count();
                    var usersToSeed = 43;
                    usersToSeed -= usersCount;

                    // ApplicationRole r = await roleManager.FindByNameAsync(standardUserRoleName);

                    for (int i = 0; i < usersToSeed; i++)
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            FirstName = faker.Name.FirstName(), LastName = faker.Name.LastName(),
                            UserName = faker.Internet.UserName(faker.Name.FirstName(), faker.Name.LastName()),
                            Email = faker.Internet.Email()
                        };
                        result = await userManager.CreateAsync(user, "password");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, standardUserRoleName);
                        }
                    }
                }
            }
        }

        private static async Task SeedAdminUserAndRole(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService configurationService = services.GetService<IConfigurationService>();
                UserManager<ApplicationUser> userManager =
                    serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                RoleManager<ApplicationRole> roleManager =
                    serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();

                string adminUserName = configurationService.GetAdminUserName();
                string adminFirstName = configurationService.GetAdminFirstName();
                string adminLastName = configurationService.GetAdminLastName();
                string adminEmail = configurationService.GetAdminEmail();
                string adminPassword = configurationService.GetAdminPassword();
                string adminRoleName = configurationService.GetAdminRoleName();
                {
                    IdentityResult authRoleCreated = IdentityResult.Success;
                    if (await roleManager.FindByNameAsync(adminRoleName) == null)
                    {
                        authRoleCreated = await roleManager.CreateAsync(new ApplicationRole(adminRoleName));
                    }

                    if (await userManager.FindByNameAsync(adminUserName) == null && authRoleCreated.Succeeded)
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            FirstName = adminFirstName,
                            LastName = adminLastName,
                            UserName = adminUserName,
                            Email = adminEmail
                        };

                        IdentityResult result = await userManager.CreateAsync(user, adminPassword);

                        if (result.Succeeded)
                        {
                            result = await userManager.AddToRoleAsync(user, adminRoleName);
                            if (!result.Succeeded)
                                throw new ThreadStateException();
                        }
                        else
                        {
                            throw new UnexpectedApplicationStateException("Failed to Create User");
                        }
                    }
                }
            }
        }

        private static async Task SeedTags(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();
                var tagCount = await dbContext.Tags.CountAsync();
                var tagsToSeed = 5;


                tagsToSeed -= tagCount;
                if (tagsToSeed <= 0)
                    return;
                var faker = new Faker<Tag>()
                    .RuleFor(t => t.Name, f => f.Lorem.Word())
                    .RuleFor(t => t.Description, f => f.Lorem.Sentences(1))
                    .FinishWith((fake, tagInstance) =>
                    {
                        var numberOfImages = fake.Random.Int(min: 1, max: 3);
                        ICollection<TagImage> fileUploads = new List<TagImage>(numberOfImages);
                        for (var i = 0; i < numberOfImages; i++)
                        {
                            var fileName = fake.System.FileName("png");
                            fileUploads.Add(new TagImage
                            {
                                OriginalFileName = fake.System.FileName("png"),
                                FileName = fileName,
                                FilePath = fake.Image.LoremPixelUrl(LoremPixelCategory
                                    .Business), //"/images/tags/" + fileName,
                                FileSize = fake.Random.Long(min: 1500, max: 20000),
                                isFeaturedImage = i == 0
                            });
                        }

                        tagInstance.TagImages = fileUploads;
                    });

                List<Tag> tags = faker.Generate(tagsToSeed);
                dbContext.Tags.AddRange(tags);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedCategories(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();
                var categoryCount = await dbContext.Categories.CountAsync();
                var categoriesToSeed = 5;
                categoriesToSeed -= categoryCount;
                if (categoriesToSeed <= 0)
                    return;
                var faker = new Faker<Category>()
                    .RuleFor(t => t.Name, f => f.Lorem.Word())
                    .RuleFor(t => t.Description, f => f.Lorem.Sentences(2));


                List<Category> categories = faker.Generate(categoriesToSeed);

                foreach (var category in categories)
                {
                    Faker fake = new Faker();

                    var numberOfImages = fake.Random.Int(min: 1, max: 3);
                    ICollection<CategoryImage> fileUploads = new List<CategoryImage>(numberOfImages);
                    for (var i = 0; i < numberOfImages; i++)
                    {
                        var fileName = fake.System.FileName("png");
                        fileUploads.Add(new CategoryImage
                        {
                            OriginalFileName = fake.System.FileName("png"),
                            FileName = fileName,
                            FilePath = fake.Image.LoremPixelUrl(LoremPixelCategory
                                .Business), // "/images/categories/" + fileName,
                            FileSize = fake.Random.Long(min: 1500, max: 20000),
                        });
                    }

                    category.CategoryImages = fileUploads;
                }

                dbContext.Categories.AddRange(categories);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedRatings(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService settingsService =
                    serviceScope.ServiceProvider.GetService<IConfigurationService>();
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();

                var faker = new Faker();
                var ratingCount = await dbContext.Ratings.CountAsync();
                var ratingSeed = 10; // seed 35 ratings (without assotiated comment)
                ratingSeed -= ratingCount;
                for (int i = ratingCount; i < ratingSeed; i++)
                {
                    ApplicationUser user = await dbContext.Users.OrderBy(a => Guid.NewGuid()).FirstAsync();
                    var product = await dbContext.Products.Include(p => p.Ratings)
                        .Where(p => p.Ratings.All(comment =>
                            comment.User != user)) // Find any whouse commenters do not include our user;
                        .FirstAsync();
                    dbContext.Ratings.Add(new Rating
                    {
                        Value = faker.Random.Int(min: 1, max: 5),
                        User = user,
                        Product = product,
                    });

                    await dbContext.SaveChangesAsync();
                }
            }
        }


        private static async Task SeedAddresses(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService settingsService =
                    serviceScope.ServiceProvider.GetService<IConfigurationService>();
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();

                // var product = await dbContext.Products.OrderBy(a => Guid.NewGuid()).FirstAsync();
                var faker = new Faker<Address>()
                    .RuleFor(c => c.StreetAddress, f => f.Address.StreetAddress())
                    .RuleFor(a => a.Country, f => f.Address.County())
                    .RuleFor(a => a.City, f => f.Address.City())
                    .RuleFor(a => a.ZipCode, f => f.Address.ZipCode())
                    .FinishWith(async (f, address) =>
                    {
                        var user = await dbContext.Users.OrderBy(a => Guid.NewGuid()).FirstAsync();

                        address.FirstName = user.FirstName;
                        address.LastName = user.LastName;
                        address.ApplicationUserId = user.Id;

                        //dbContext.Attach(c.User);
                    });

                var addressesCount = await dbContext.Addresses.CountAsync();
                var commentsToSeed = 35;
                commentsToSeed -= addressesCount;

                if (commentsToSeed > 0)
                {
                    List<Address> addresses = faker.Generate(commentsToSeed);
                    dbContext.Addresses.AddRange(addresses);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedComments(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService settingsService =
                    serviceScope.ServiceProvider.GetService<IConfigurationService>();
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();

                // var product = await dbContext.Products.OrderBy(a => Guid.NewGuid()).FirstAsync();
                var faker = new Faker<Comment>()
                    .RuleFor(c => c.Content, f => f.Lorem.Sentences(f.Random.Number(1, 3)))
                    .FinishWith(async (f, c) =>
                    {
                        if (f.Random.Bool(0.75f))
                            c.Rating = f.Random.Int(min: 1, max: 5);


                        var user = await dbContext.Users.OrderBy(a => Guid.NewGuid()).FirstAsync();

                        // Random Product not rated already by our User
                        Product product = await dbContext.Products.Include(p => p.Comments)
                            .OrderBy(a => Guid.NewGuid()).FirstAsync();
                        /* .Where(p => p.Comments.All(comment =>
                                comment.Rating.User != user)) // Find any whose comment authors do not include our user
                            .OrderBy(a => Guid.NewGuid())
                            .FirstAsync();
                        c.Rating = new Rating
                        {
                            Comment = c, User = user, Value = f.Random.Int(min: 1, max: 5),
                            Product = product
                        };
                        */
                        c.Product = product;
                        c.User = user;
                        //dbContext.Attach(c.User);
                    });

                var commentsCount = await dbContext.Comments.CountAsync();
                var commentsToSeed = 35;
                commentsToSeed -= commentsCount;

                if (commentsToSeed > 0)
                {
                    List<Comment> comments = faker.Generate(commentsToSeed);
                    dbContext.Comments.AddRange(comments);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedProducts(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService settingsService =
                    serviceScope.ServiceProvider.GetService<IConfigurationService>();
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();
                var productsCount = await dbContext.Products.CountAsync();
                var productsToSeed = 35;
                productsToSeed -= productsCount;
                if (productsToSeed <= 0)
                    return;

                UserManager<ApplicationUser> userManager =
                    serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                RoleManager<ApplicationRole> roleManager =
                    serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();

                var faker = new Faker<Product>()
                    .RuleFor(a => a.PublishAt, f => f.Date
                        .Between(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(1)))
                    .RuleFor(a => a.Name, f => f.Commerce.ProductName()) //  f.Lorem.Sentence()
                    .RuleFor(a => a.Description, f => f.Lorem.Sentences(2))
                    .RuleFor(p => p.Price,
                        f => f.Random.Int(min: 50,
                            max: 1000)) // f.Commerce.Price(min: 50, max: 1000) will return a string
                    .RuleFor(p => p.Stock, f => f.Random.Int(min: 0, max: 2500))
                    .FinishWith(async (f, aproductInstance) =>
                    {
                        ICollection<ProductTag> productTags = new List<ProductTag>();
                        productTags.Add(new ProductTag
                            {
                                Product = aproductInstance,
                                ProductId = aproductInstance.Id,
                                Tag = await dbContext.Tags.OrderBy(t => Guid.NewGuid()).FirstAsync()
                            }
                        );
                        aproductInstance.ProductTags = productTags;

                        ICollection<ProductCategory> productCategories = new List<ProductCategory>();
                        productCategories.Add(new ProductCategory
                            {
                                Product = aproductInstance,
                                ProductId = aproductInstance.Id,
                                Category = await dbContext.Categories.OrderBy(t => Guid.NewGuid()).FirstAsync()
                            }
                        );
                        aproductInstance.ProductCategories = productCategories;

                        aproductInstance.Slug = aproductInstance.Name.Slugify();


                        var numberOfImages = f.Random.Int(min: 1, max: 3);
                        ICollection<ProductImage> fileUploads = new List<ProductImage>(numberOfImages);
                        for (var i = 0; i < numberOfImages; i++)
                        {
                            var fileName = f.System.FileName("png");
                            fileUploads.Add(new ProductImage
                            {
                                OriginalFileName = f.System.FileName("png"),
                                FileName = fileName,
                                FilePath = f.Image.LoremPixelUrl(LoremPixelCategory
                                    .Business), // "/images/products/" + fileName,
                                FileSize = f.Random.Long(min: 1500, max: 20000),
                                isFeaturedImage = i == 0
                            });
                        }

                        aproductInstance.ProductImages = fileUploads;
                    });


                List<Product> products = faker.Generate(productsToSeed);
                products.ForEach(a =>
                {
                    dbContext.Products.Add(a);
                    //       dbContext.Entry(a).State = EntityState.Added;
                });
                EntityEntry<Product> entry = dbContext.Products.Add(products[0]);
                dbContext.Products.AddRange(products);
                // dbContext.ChangeTracker.DetectChanges();
                // var res = dbContext.SaveChanges();
                await dbContext.SaveChangesAsync();
            }
        }


        public static async Task SeedOrders(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService settingsService =
                    serviceScope.ServiceProvider.GetService<IConfigurationService>();
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();

                Array values = Enum.GetValues(typeof(ShippingStatus));
                Random random = new Random();


                var faker = new Faker<Order>()
                    .RuleFor(o => o.TrackingNumber, f => f.Random.AlphaNumeric(16))
                    .RuleFor(o => o.OrderStatus, f => (ShippingStatus) f.Random.Int(min: 1, max: values.Length))
                    .FinishWith(async (fk, order) =>
                    {
                        // User
                        var orderingUser =
                            fk.Random.Bool(
                                0.75f) // 75% change we create an order with an authenticated user, 25% change of guest user making the order
                                ? await dbContext.Users.Include(u => u.Addresses).OrderBy(a => Guid.NewGuid())
                                    .FirstAsync()
                                : null;
                        order.User = orderingUser;


                        // Address
                        if (orderingUser?.Addresses?.Count > 0)
                        {
                            if (orderingUser.Addresses?.Count > 0 && orderingUser.Addresses.GetType() == typeof(IList))
                                order.Address =
                                    ((IList<Address>) orderingUser.Addresses)[
                                        fk.Random.Int(min: 0, max: orderingUser.Addresses.Count)];
                            else
                                order.Address = await dbContext.Addresses.Where(a => a.User == orderingUser)
                                    .OrderBy(e => Guid.NewGuid())
                                    .FirstAsync();
                        }
                        else
                        {
                            order.Address = new Address
                            {
                                // we may have a user but with 0 Addresses
                                ApplicationUserId = orderingUser?.Id,
                                FirstName = fk.Name.FirstName(),
                                LastName = fk.Name.LastName(),
                                StreetAddress = fk.Address.StreetAddress(),
                                City = fk.Address.City(),
                                Country = fk.Address.Country(),
                                ZipCode = fk.Address.ZipCode()
                            };
                        }


                        // Seed OrderItems
                        var product = dbContext.Products.OrderBy(p => Guid.NewGuid()).First();

                        // OrderItems
                        ICollection<OrderItem> orderItems = new List<OrderItem>();
                        for (int i = 0; i < fk.Random.Int(min: 1, max: 20); i++)
                        {
                            orderItems.Add(new OrderItem
                            {
                                User = orderingUser,
                                Order = order,
                                // OrderId = order.Id,
                                Slug = product.Slug,
                                Name = product.Name,
                                Product = await dbContext.Products.OrderBy(p => Guid.NewGuid()).FirstAsync(),
                                Price = Math.Max(10, fk.Random.Int(min: -20, max: 20) + product.Price),
                                Quantity = fk.Random.Int(min: 1, max: 10)
                            });
                        }

                        order.OrderItems = orderItems;
                        //dbContext.Attach(c.User);
                    });

                var ordersCount = await dbContext.Orders.CountAsync();
                var ordersToSeed = 35;
                ordersToSeed -= ordersCount;

                if (ordersToSeed > 0)
                {
                    List<Order> orders = faker.Generate(ordersToSeed);
                    dbContext.Orders.AddRange(orders);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public static async Task Seed(IServiceProvider services)
        {
            await SeedAdminUserAndRole(services);
            await SeedAuthenticatedUsersAndRole(services);
            await SeedTags(services);
            await SeedCategories(services);
            await SeedProducts(services);
            await SeedComments(services);
            // await SeedRatings(services);
            await SeedAddresses(services);
            await SeedOrders(services);
        }
    }
}
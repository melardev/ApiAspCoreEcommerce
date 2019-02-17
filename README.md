# Introduction
This is one of my E-commerce API app implementations. It is written in .Net C Sharp using ASP.Net Core MVC framework.
This is not a finished project by any means, but it has a valid enough shape to be git cloned and studied if you are interested in this topic.
If you are interested in this project take a look at my other server API implementations I have made with:

- [Node Js + Sequelize](https://github.com/melardev/ApiEcomSequelizeExpress)
- [Node Js + Bookshelf](https://github.com/melardev/ApiEcomBookshelfExpress)
- [Node Js + Mongoose](https://github.com/melardev/ApiEcomMongooseExpress)
- [Python Django](https://github.com/melardev/DjangoRestShopApy)
- [Python Flask]()
- [Golang go gonic]()
- [Java EE Spring Boot and Hibernate](https://github.com/melardev/SBootApiEcomMVCHibernate)
- [Ruby on Rails](https://github.com/melardev/RailsApiEcommerce)
- [Laravel]()



# Getting Started
1. Git clone the project
2. Run reset_db.bat or execute each one of his commands to reset the database migrations,
or if you prefer to just go ahead and migrate with the given migration files.
3. Run the app, the application will automatically try to seed the database if there are no enough records, 
most likely the app will crash because Bogus(Faker for dotnet) generates product names for seeding that may not be unique, since there is a UNIQUE constraint
on the database the app may crash, in that case run it again, hopefully this time will generate a product name that is not in the database yet.
4. You can import the api.postman_collection.json into postman to make the requests by yourself

# Useful commands
- Create Initial Migration
`dotnet ef migrations add InitialCreate`
- Migrate
`dotnet ef database update`
I always prefer to use dotnet instead of Nuget Console. But If you
want the equivalent Nuget console commands then:
`Add-Migration InitialCreate`
`Update-Database`



# TODO:
- The Jwt middleware that ships with Asp.Net Core is fine, but it does not validate if the user actually exists, it only checks if the Jwt is valid, I have to create
a middleware that does that validation, or at least, make sure user is not null when retrieving it from IUserService.GetCurrentUserAsync()
- Refractoring, there are some repeated code, check if it can be placed in a common place
- A lot of refactoring related to move files to appropriate namespaces, useful comments, cleanup code.
- Admin features
- Benchmark middleware or filter that prints in the console how much time it took the request to get the response
- Unique clauses for slugs and role.name
- User profile feature
- 
- Improving database performance, there are some queries made that retrieve more data than used
- Improve performance by telling EF Core which model entry has its state changed: added/modified/deleted, instead of calling SaveChangesAsync() for each change
- Also related with security, I have to review the authorization(access control) to actions
- Rethink the Comment model(Rating, replies, etc.)
- Change CORS from allowing any to allow origins, methods and headers configured in settings json file
- There is a lack of  validations and security checks in:
    - OriginalFileName, (I have to research if this can lead to some kind of SQLi)
    - Reflected XSS: comments, user's data(username, firstName, etc.)
    - LFI: nots sure for now, review later.
    - Not implemented yet access control for tags and categories management
    - Every single input has to be validated, the code is trusting a lot, I have to add checks pretty much everywhere
- App Settings:
    - The admin should be able to decide if user has to validate email registration or not
    - I have to test if svg uploads may lead to XSS and how to prevent them
     
# Resources
- [SQLi and ASP.NET Core](https://adrientorris.github.io/entity-framework-core/SQL-Injection-attacks-in-Entity-Framework-Core-2-0.html)
- [Prevent XSS](https://docs.microsoft.com/en-us/aspnet/core/security/cross-site-scripting?view=aspnetcore-2.2)
 
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ApiCoreEcommerce.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCoreEcommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            EncodingProvider ins = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ins);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    IHostingEnvironment env = services.GetRequiredService<IHostingEnvironment>();
                    IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();

                    if (!ApplicationDbContext.Seed(services, config).Result.IsCompletedSuccessfully)
                        Environment.Exit(-1);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // .UseKestrel(kerselOptions => kerselOptions.AddServerHeader = false)
                // .UseContentRoot(Directory.GetCurrentDirectory())
                // .UseIISIntegration()
                .UseUrls("http://localhost:8080" /*, "https://localhost:5001"*/)
                .UseStartup<Startup>();
    }
}
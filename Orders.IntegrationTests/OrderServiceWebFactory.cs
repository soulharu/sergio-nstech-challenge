using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orders.Domain.Entities;
using Orders.Infrastructure.Persistence;

namespace Orders.IntegrationTests
{
    public class OrderServiceWebFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "TestDb_" + Guid.NewGuid();
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddControllers()
                .AddApplicationPart(typeof(Program).Assembly);

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<OrdersDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<OrdersDbContext>(options =>
                    options.UseInMemoryDatabase(_dbName));

                var sp = services.BuildServiceProvider();
            });

            builder.UseEnvironment("Development");
        }

        public void SeedDatabase()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            db.Database.EnsureCreated();

            if (db.Products.Any()) return;

            db.Products.AddRange(
                new Product(Product1Id, "Product A", 29.20m, 50),
                new Product(Product2Id, "Product B", 200m, 10));
            db.SaveChanges();
        }

        public static readonly Guid Product1Id = Guid.Parse("07f763d0-4acb-41c9-b4bc-23e3f53c73c9");
        public static readonly Guid Product2Id = Guid.Parse("e6c0f08b-30f9-4e0e-9b4a-ecce3ca98f41");
    }
}

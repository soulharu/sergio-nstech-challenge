using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Persistence
{
    public static class DbSeed
    {
        public static async Task SeedDbAsync(OrdersDbContext context)
        {
            //await context.Database.MigrateAsync();

            if (await context.Products.AnyAsync()) return;

            var products = new List<Product>
        {
            new(Guid.Parse("07f763d0-4acb-41c9-b4bc-23e3f53c73c9"), "Dados de RPG - Conjunto com 7 Dados Translúcidos - Transparent Blue", 29.20m, 80),
            new(Guid.Parse("e6c0f08b-30f9-4e0e-9b4a-ecce3ca98f41"), "Kit de Pincéis Alkimya para Detalhamento - Kolinsky Pure Hair", 419.90m, 10),
            new(Guid.Parse("23c87b01-2163-40a0-bf13-69d23164e994"), "Lata de Verniz em Spray - Brilhante 300ml", 59.50m, 50),
            new(Guid.Parse("0ad91967-9d83-4137-851a-56329930d83b"), "Diário de anotações em Couro - Azul", 65.00m, 10),
            new(Guid.Parse("247df04f-0a77-4677-a1a8-91362a6bb3e3"), "Bandeja de Dados em Couro com Revestimento Aveludado - Preto/Vermelho", 42.30m, 40),
        };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}

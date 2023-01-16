using OrderService.Models;
using System;

namespace OrderService.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbInitializer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<OrdersContext>())
                {
                   
                }
            }
        }

        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<OrdersContext>())
                {

                    //add admin user
                    if (!context.Products.Any())
                    {
                        context.Products.AddRange(new List<Product>(){
                            new Product() { Id = 1, Price = 10, Title = "Product 1" },
                            new Product() { Id = 2, Price = 4.5m, Title = "Product 2" },
                            new Product() { Id = 3, Price = 15.2m, Title = "Product 3" }
                        });
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}

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
                using (var context = serviceScope.ServiceProvider.GetService<AppDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<AppDbContext>())
                {

                    //add admin user
                    if (!context.Users.Any())
                    {
                        var adminUser = new User
                        {
                            IsActive = true,
                            Username = "admin",
                            Password = "admin1234", // should be hash
                            SerialNumber = Guid.NewGuid().ToString()
                        };
                        context.Users.Add(adminUser);
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}

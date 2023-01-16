using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace OrderService.Data
{
    public class OrdersContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "OrdersDB");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasOne(s => s.Order)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(s => s.OrderId);

            //

            modelBuilder.Entity<Product>().HasData(
                new Product() { Id = 1, Price = 10, Title = "Product 1" },
                new Product() { Id = 2, Price = 4.5m, Title = "Product 2" },
                new Product() { Id = 3, Price = 15.2m, Title = "Product 3" });

        }


    }
}

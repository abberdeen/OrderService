using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace OrderService
{
    public class OrdersContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasOne(s => s.Order)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(s => s.OrderId);
        }

    }
}

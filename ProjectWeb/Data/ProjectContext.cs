using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectWeb.Models;
using ProjectWeb.Areas.Identity.Data;

namespace ProjectWeb.Data
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasOne<UserData>(s => s.User)
                .WithMany(g => g.Orders)
                .HasForeignKey(s => s.CustomerID);

            modelBuilder.Entity<Cart>()
             .HasOne<Order>(s => s.OrderItem)
             .WithOne(a => a.CartItems)
              .HasForeignKey<Cart>(g => g.OrderRef);

            modelBuilder.Entity<Product>()
             .HasMany(s => s.Cart)
             .WithOne(p => p.ProductItem)
             .HasForeignKey(s => s.ProductRef);

            modelBuilder.Entity<Cart>()
                .HasMany<CartUser>(s => s.ItemsInCart)
                .WithOne(s => s.ShoppingCart)
                .HasForeignKey(g => g.CartItemRef);
        }
         public DbSet<Product> Product { get; set; }
         public DbSet<UserData> UserData { get; set; }
         public DbSet<Order> Order { get; set; }
         public DbSet<CartUser> CartUser { get; set; }
         public DbSet<Cart> Cart { get; set; }

    }
}

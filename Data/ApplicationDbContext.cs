using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EcommerceApp.Models;

namespace EcommerceApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Description = "High-performance laptop with 16GB RAM",
                    Price = 999.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?q=80&w=2071&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
                },
                new Product
                {
                    Id = 2,
                    Name = "Smartphone",
                    Description = "Latest smartphone with 128GB storage",
                    Price = 699.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1598965402089-897ce52e8355?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTF8fHNtYXJ0cGhvbmV8ZW58MHx8MHx8fDA%3D"
                },
                new Product
                {
                    Id = 3,
                    Name = "Headphones",
                    Description = "Wireless noise-cancelling headphones",
                    Price = 199.99m,
                    ImageUrl = "https://plus.unsplash.com/premium_photo-1679513691474-73102089c117?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8aGVhZHBob25lfGVufDB8fDB8fHww"
                },
                new Product
                {
                    Id = 4,
                    Name = "Smartwatch",
                    Description = "Fitness tracking smartwatch with heart rate monitor",
                    Price = 249.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1734776579769-4fbfcdd12b6a?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8N3x8d2F0Y2hzbWFydHxlbnwwfHwwfHx8MA%3D%3D"
                },
                new Product
                {
                    Id = 5,
                    Name = "Tablet",
                    Description = "10-inch tablet with 64GB storage",
                    Price = 499.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1542751110-97427bbecf20?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8NHx8dGFibGV0fGVufDB8fDB8fHww"
                },
                new Product
                {
                    Id = 6,
                    Name = "Camera",
                    Description = "Digital camera with 24MP sensor",
                    Price = 799.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1519638831568-d9897f8109a0?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.0.3"
                },
                new Product
                {
                    Id = 7,
                    Name = "Gaming Console",
                    Description = "Next-gen gaming console with 1TB storage",
                    Price = 499.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1606144042614-b040efbe5f94?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.0.3"
                });

            builder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId);

            builder.Entity<Rating>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId);

            builder.Entity<Wishlist>()
                .HasOne(w => w.Product)
                .WithMany()
                .HasForeignKey(w => w.ProductId);
        }
    }
}
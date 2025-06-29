using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Lavender_Veil.Models;

namespace Lavender_Veil.Models
{
    public class ApplicationDbContext : IdentityDbContext<Customer>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Size> Sizes { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Skirts" },
                new Category { CategoryId = 2, Name = "Accessories" },
                new Category { CategoryId = 3, Name = "Hijabs" },
                new Category { CategoryId = 4, Name = "Swimwear" },
                new Category { CategoryId = 5, Name = "Abayas" },
                new Category { CategoryId = 6, Name = "Tops" },
                new Category { CategoryId = 7, Name = "Sets" },
                new Category { CategoryId = 8, Name = "Hijab_accessories" },
                new Category { CategoryId = 9, Name = "Dresses" },
                new Category { CategoryId = 10, Name = "Robes" },
                new Category { CategoryId = 11, Name = "Prayer_sets" }
            );
            modelBuilder.Entity<Size>().HasData(
                new Size { SizeId = 1, Name = "XS" },
                new Size { SizeId = 2, Name = "S" },
                new Size { SizeId = 3, Name = "M" },
                new Size { SizeId = 4, Name = "L" },
                new Size { SizeId = 5, Name = "XL" },
                new Size { SizeId = 6, Name = "One Size" }
            );


            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId);
        }
        
    }
}

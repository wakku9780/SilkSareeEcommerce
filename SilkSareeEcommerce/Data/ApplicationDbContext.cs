using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }  // ✅ Order ke products store honge
        public DbSet<CartItem> CartItems { get; set; }    // ✅ Shopping Cart system ke liye
        public DbSet<Wishlist> Wishlists { get; set; }    // ✅ Wishlist system ke liye
        public DbSet<Category> Categories { get; set; }  // ✅ Sarees ki categories store hongi
        public DbSet<Payment> Payments { get; set; }    // ✅ Payment transactions ke liye

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // OrderItem relationship
            builder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(o => o.OrderId);

            // CartItem relationship
            builder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId);

            // Wishlist relationship
            builder.Entity<Wishlist>()
                .HasOne(w => w.Product)
                .WithMany()
                .HasForeignKey(w => w.ProductId);
        }
    }
}

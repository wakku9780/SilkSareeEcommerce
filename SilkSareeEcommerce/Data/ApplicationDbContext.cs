using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<SavedAddress> SavedAddresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }  // ✅ Order ke products store honge
        public DbSet<CartItem> CartItems { get; set; }    // ✅ Shopping Cart system ke liye
        public DbSet<Wishlist> Wishlists { get; set; }    // ✅ Wishlist system ke liye
        public DbSet<Category> Categories { get; set; }  // ✅ Sarees ki categories store hongi
        public DbSet<Payment> Payments { get; set; }    // ✅ Payment transactions ke liye
        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Coupon> Coupons { get; set; }

        public DbSet<UserCoupon> UserCoupons { get; set; }



        public DbSet<ProductReview> ProductReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<UserCoupon>()
        .HasIndex(uc => new { uc.UserId, uc.CouponId })
        .IsUnique(); // Prevent multiple uses


            builder.Entity<Product>()
         .HasMany(p => p.Orders)
         .WithMany(o => o.Products); // Many-to-Many relationship


            builder.Entity<Order>()
        .HasMany(o => o.Products)
        .WithMany(p => p.Orders)
        .UsingEntity(j => j.ToTable("OrderProducts"));  // Many-to-many relationship



            // OrderItem relationship
            builder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(o => o.OrderId);



            // Order entity configuration
            builder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            // OrderItem to Product mapping
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);




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

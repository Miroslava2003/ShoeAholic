using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoeAholic.Areas.Identity.Data;
using ShoeAholic.Models;

namespace ShoeAholic.Data
{
    public class ShoeAholicDbContext : IdentityDbContext<ApplicationUser>
    {
        public ShoeAholicDbContext(DbContextOptions<ShoeAholicDbContext> options)
            : base(options)
        {
        }

        public DbSet<Shoe> Shoes { get; set; } = null!;
        public DbSet<ShoeSize> ShoeSizes { get; set; } = null!;
        public DbSet<ShoeImage> ShoeImages { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = null!;
        public DbSet<WishlistItem> WishlistItems { get; set; } = null!;
        public DbSet<ContactMessage> ContactMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Shoe>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            builder.Entity<ShoeSize>()
                .Property(s => s.Size)
                .HasPrecision(4, 1);

            builder.Entity<Order>()
                .Property(o => o.SubTotal)
                .HasPrecision(10, 2);

            builder.Entity<Order>()
                .Property(o => o.DeliveryPrice)
                .HasPrecision(10, 2);

            builder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(10, 2);

            builder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(10, 2);

            builder.Entity<ShoppingCartItem>()
                .HasOne(sci => sci.Shoe)
                .WithMany()
                .HasForeignKey(sci => sci.ShoeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ShoppingCartItem>()
                .HasOne(sci => sci.ShoeSize)
                .WithMany()
                .HasForeignKey(sci => sci.SizeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Shoe)
                .WithMany()
                .HasForeignKey(oi => oi.ShoeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<WishlistItem>()
                .HasOne(w => w.Shoe)
                .WithMany()
                .HasForeignKey(w => w.ShoeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
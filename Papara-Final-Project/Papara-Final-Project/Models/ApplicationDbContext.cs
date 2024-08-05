using Microsoft.EntityFrameworkCore;

namespace Papara_Final_Project.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductMatchCategory> ProductMatchCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductMatchCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductMatchCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductMatchCategories)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductMatchCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductMatchCategories)
                .HasForeignKey(pc => pc.CategoryId);

            modelBuilder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .IsUnique();
        }
    }
}

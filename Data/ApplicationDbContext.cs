using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Shop111.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<ProductInformation> ProductInformations { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ১. ApplicationUser (Identity) এবং Vendor-এর মধ্যকার ওয়ান-টু-ওয়ান রিলেশন
            builder.Entity<Vendor>()
                .HasOne(v => v.User)
                .WithOne(u => u.Vendor)
                .HasForeignKey<Vendor>(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade); // ইউজার ডিলিট হলে ভেন্ডর প্রোফাইলও ডিলিট হবে

            // ২. Vendor এবং Location (Many-to-One)
            builder.Entity<Vendor>()
                .HasOne(v => v.Location)
                .WithMany(l => l.Vendors)
                .HasForeignKey(v => v.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ৩. Product এবং ApplicationUser (Vendor হিসেবে)
            builder.Entity<Product>()
                .HasOne(p => p.Vendor)
                .WithMany(v => v.Products)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            // ৪. OrderDetail এবং ApplicationUser
            builder.Entity<OrderDetail>()
                .HasOne(od => od.Vendor)
                .WithMany() // যদি ভেন্ডর মডেলে OrderDetails লিস্ট না থাকে তবে এটি খালি থাকবে
                .HasForeignKey(od => od.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ৫. CartDetail এবং ApplicationUser
            builder.Entity<CartDetail>()
                .HasOne(cd => cd.Vendor)
                .WithMany()
                .HasForeignKey(cd => cd.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ৬. Stock এবং Product (One-to-One)
            builder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithOne(p => p.Stock)
                .HasForeignKey<Stock>(s => s.ProductId);
            
        }
    }
}
using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Data
{
    
    public class ProductDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Type).IsRequired();
                entity.HasOne(p => p.Inventory)
                    .WithOne() 
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.Price).IsRequired();
                entity.Property(p => p.ExpirationDate).IsRequired(); 
            });
        }
    }
    
}


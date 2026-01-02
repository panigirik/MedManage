using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Data
{

    public class InventoryDbContext : DbContext
    {
        public DbSet<Inventory> Inventories { get; set; }

        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Inventory>(entity =>
            {
                
                entity.Property(i => i.QuantityInStock).IsRequired();
                entity.Property(i => i.LastUpdated).IsRequired();


                entity.HasOne(i => i.Product)
                    .WithOne(p => p.Inventory) // Предполагается, что в сущности Product есть свойство Inventory
                    .HasForeignKey<Inventory>(i => i.ProductId) // Укажите внешний ключ
                    .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}
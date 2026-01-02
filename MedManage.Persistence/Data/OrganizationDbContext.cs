using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Data
{
    public class OrganizationDbContext : DbContext
    {
        public DbSet<Organization> Organizations { get; set; }

        public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Конфигурируем Organization
            modelBuilder.Entity<Organization>()
                .Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Organization>()
                .Property(o => o.Address)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Organization>()
                .Property(o => o.PhoneNumber)
                .HasMaxLength(15);

            modelBuilder.Entity<Organization>()
                .Property(o => o.Email)
                .HasMaxLength(100);

            modelBuilder.Entity<Organization>()
                .Property(o => o.CreatedAt)
                .IsRequired();
        }
    }
}
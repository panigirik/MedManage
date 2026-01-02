using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Data
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.FullName).HasMaxLength(100);
                entity.Property(u => u.CreatedAt).IsRequired();
                

            });
            
            
        }
    }
}
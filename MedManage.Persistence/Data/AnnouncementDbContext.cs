using MedManage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Data
{
    public class AnnouncementDbContext : DbContext
    {
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<User> User { get; set; } // Добавлено для работы с пользователями

        public AnnouncementDbContext(DbContextOptions<AnnouncementDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.HasKey(a => a.AnnouncementId);

                entity.Property(a => a.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(a => a.Content)
                    .IsRequired(false)
                    .HasMaxLength(2000);

                entity.Property(a => a.CreatedAt)
                    .IsRequired();

                entity.Property(a => a.UpdatedAt)
                    .IsRequired();

                entity.Property(a => a.ExpirationDate)
                    .IsRequired(false);

                entity.Property(a => a.StatusInventory)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(a => a.TypeProduct)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(a => a.Views)
                    .IsRequired(true)
                    .HasConversion<int>();

                // Связь с User через CreatedByUserId
                entity.HasOne(a => a.User)
                    .WithMany() // Один пользователь может иметь несколько объявлений
                    .HasForeignKey(a => a.CreatedByUserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict); // Удаление пользователя не затрагивает объявления
            });


            modelBuilder.Entity<User>(entity =>
            {

                entity.Property(u => u.UserName)
                    .IsRequired()
                    .HasMaxLength(64); // Максимальный размер 64 символа
                
            });
        }
    }
}

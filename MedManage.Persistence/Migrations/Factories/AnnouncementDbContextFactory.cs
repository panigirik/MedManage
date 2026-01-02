using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MedManage.Persistence.Migrations.Factories
{
    public class AnnouncementDbContextFactory : IDesignTimeDbContextFactory<AnnouncementDbContext>
    {
        public AnnouncementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AnnouncementDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=MedManageDB;Username=postgres;Password=Volvos80");

            return new AnnouncementDbContext(optionsBuilder.Options);
        }
    }
}
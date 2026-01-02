using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MedManage.Persistence.Migrations.Factories
{
    
    public class AdminDdContextFactory: IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=MedManageDB;Username=postgres;Password=Volvos80");

            return new UserDbContext(optionsBuilder.Options);
        }
    }
    
}
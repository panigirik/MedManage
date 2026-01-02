
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MedManage.Persistence.Migrations.Factories
{
    
    public class InventoryDdContextFactory: IDesignTimeDbContextFactory<OrganizationDbContext>
    {
        public OrganizationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrganizationDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=MedManageDB;Username=postgres;Password=Volvos80");

            return new OrganizationDbContext(optionsBuilder.Options);
        }
    }
    
}


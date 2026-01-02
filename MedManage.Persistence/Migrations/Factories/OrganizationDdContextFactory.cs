using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using MedManage.Persistence.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace MedManage.Persistence.Migrations.Factories
{
    public class OrganizationDdContextFactory: IDesignTimeDbContextFactory<OrganizationDbContext>
    {
        public OrganizationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrganizationDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=MedManageDB;Username=postgres;Password=Volvos80");

            return new OrganizationDbContext(optionsBuilder.Options);
        }
    }
}


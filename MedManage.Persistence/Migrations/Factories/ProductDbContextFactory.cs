using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MedManage.Persistence.Migrations.Factories
{
    
    public class ProductDbContextFactory: IDesignTimeDbContextFactory<ProductDbContext>
    {
        public ProductDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=MedManageDB;Username=postgres;Password=Volvos80");

            return new ProductDbContext(optionsBuilder.Options);
        }
    }
    
}
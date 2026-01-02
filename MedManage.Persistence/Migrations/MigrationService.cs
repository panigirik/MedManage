using Microsoft.EntityFrameworkCore;
using MedManage.Persistence.Data;


namespace MedManage.Infrastructure.Migrations
{
    public class MigrationService
    {
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly UserDbContext _userDbContext;
        private readonly ProductDbContext _productDbContext;
        private readonly InventoryDbContext _inventoryDbContext;
        private readonly AnnouncementDbContext _announcementDbContext;

        public MigrationService(
            OrganizationDbContext organizationDbContext,
            UserDbContext userDbContext,
            ProductDbContext productDbContext,
            InventoryDbContext inventoryDbContext,
            AnnouncementDbContext announcementDbContext)
        {
            _organizationDbContext = organizationDbContext;
            _userDbContext = userDbContext;
            _productDbContext = productDbContext;
            _inventoryDbContext = inventoryDbContext;
            _announcementDbContext = announcementDbContext;
        }

        public void MigrateAll()
        {
            _organizationDbContext.Database.Migrate();
            _userDbContext.Database.Migrate();
            _productDbContext.Database.Migrate();
            _inventoryDbContext.Database.Migrate();
            _announcementDbContext.Database.Migrate();
            
        }
    }
}
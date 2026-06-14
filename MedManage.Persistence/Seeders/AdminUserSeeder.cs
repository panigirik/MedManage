using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Seeders;

public class AdminUserSeeder : IDataSeeder
{
    public const string DefaultUserName = "admin";
    public const string DefaultPassword = "Admin123!";
    public const string DefaultFullName = "System Administrator";
    public const string DefaultPhoneNumber = "+79000000000";

    private readonly AppDbContext _context;

    public AdminUserSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var adminExists = await _context.Users
            .AnyAsync(u => u.UserName == DefaultUserName, cancellationToken);

        if (adminExists)
        {
            return;
        }

        var admin = new User(
            DefaultUserName,
            DefaultFullName,
            UserRole.Admin,
            DefaultPhoneNumber);

        await _context.Users.AddAsync(admin, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

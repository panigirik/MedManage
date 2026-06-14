using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using MedManage.Persistence.Transactions;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью пользователей.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IAppDbContext _context;

    public UserRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    [Transactional]
    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    [Transactional]
    public async Task<User> AddAsync(
        string userName,
        string fullName,
        UserRole role,
        string phoneNumber,
        Guid? organizationId = null)
    {
        var user = new User(userName, fullName, role, phoneNumber, organizationId);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }
}

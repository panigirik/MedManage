using MedManage.Domain.Entities;
using MedManage.Domain.Enums;

namespace MedManage.Domain.Interfaces;

/// <summary>
/// Интерфейс репозитория для работы с пользователями.
/// </summary>
public interface IUserRepository
{
    Task<User> GetUserByIdAsync(Guid userId);

    Task<IEnumerable<User>> GetAllUsersAsync();

    Task<User> GetByIdAsync(Guid userId);

    Task<User> AddAsync(
        string userName,
        string fullName,
        UserRole role,
        string phoneNumber,
        Guid? organizationId = null);

    Task UpdateAsync(User user);
}

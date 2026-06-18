using MedManage.Application.DTOs;
using MedManage.Domain.Enums;

namespace MedManage.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с пользователями.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Получить всех пользователей, кроме текущего.
    /// </summary>
    Task<IEnumerable<UserDTO>> GetAllUsersExceptAsync();

    /// <summary>
    /// Обновить информацию о пользователе.
    /// </summary>
    Task UpdateUserInfoAsync(UserDTO updatedUser);

    Task UpdateUserRoleAsync(UserDTO updatedUser, UserRole newRole);

    Task<UserDTO> GetCurrentUserAsync();
    
    /// <summary>
    /// Получить имя пользователя из токена.
    /// </summary>
    string GetUserNameFromToken();

    Task UpdateUserPhoneNumberAsync(UserDTO updatedUser);
}
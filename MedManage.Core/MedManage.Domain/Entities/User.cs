using MedManage.Domain.Enums;

namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность пользователя.
/// </summary>
public class User
{
    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Роль пользователя в системе.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Дата и время создания записи о пользователе.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    public string PhoneNumber { get; set; }
}
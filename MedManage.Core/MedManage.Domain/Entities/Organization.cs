namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность организации.
/// </summary>
public class Organization
{
    /// <summary>
    /// Уникальный идентификатор организации.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Название организации.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Адрес организации.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Контактный номер телефона.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Контактный email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Дата и время создания записи об организации.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
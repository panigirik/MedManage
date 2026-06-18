namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность организации (медицинское учреждение, поставщик и т.д.).
/// </summary>
public class Organization
{
    private Organization()
    {
    }

    public Organization(string name, string address, string phoneNumber, string email)
    {
        OrganizationId = Guid.NewGuid();
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Уникальный идентификатор организации.
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// Название организации.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Адрес организации.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Контактный номер телефона.
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Контактный email.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Дата и время создания записи об организации.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Пользователи, принадлежащие организации.
    /// </summary>
    public ICollection<User> Users { get; private set; } = new List<User>();

    /// <summary>
    /// Продукты, принадлежащие организации.
    /// </summary>
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    /// <summary>
    /// Объявления, относящиеся к организации.
    /// </summary>
    public ICollection<Announcement> Announcements { get; private set; } = new List<Announcement>();
}

using MedManage.Domain.Enums;

namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность объявления.
/// </summary>
public class Announcement
{
    /// <summary>
    /// Уникальный идентификатор объявления.
    /// </summary>
    public Guid AnnouncementId { get; set; }

    /// <summary>
    /// Заголовок объявления.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Содержимое объявления.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Дата и время создания объявления.
    /// </summary>
    public DateTimeOffset  CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата и время истечения объявления (если задано).
    /// </summary>
    public DateTimeOffset ? ExpirationDate { get; set; }

    /// <summary>
    /// Уникальный идентификатор пользователя, создавшего объявление.
    /// </summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Статус инвентаризации.
    /// </summary>
    public InventoryStatus StatusInventory { get; set; }

    /// <summary>
    /// Тип продукта, связанный с объявлением.
    /// </summary>
    public ProductType TypeProduct { get; set; }

    /// <summary>
    /// Дата и время последнего обновления объявления.
    /// </summary>
    public DateTimeOffset  UpdatedAt { get; set; } = DateTime.UtcNow; 

    /// <summary>
    /// Навигационное свойство: пользователь, создавший объявление.
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Имя пользователя, создавшего объявление.
    /// </summary>
    public string UserName { get; set; }
    
    /// <summary>
    /// Количество просмотров объявления.
    /// </summary>
    public int Views { get; set; }
}
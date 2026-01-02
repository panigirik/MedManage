namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность инвентаризации.
/// </summary>
public class Inventory
{
    /// <summary>
    /// Количество товара на складе.
    /// </summary>
    public int QuantityInStock { get; set; }

    /// <summary>
    /// Дата и время последнего обновления инвентаризации.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Уникальный идентификатор продукта (внешний ключ).
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Навигационное свойство: продукт, связанный с инвентаризацией.
    /// </summary>
    public Product Product { get; set; }
}
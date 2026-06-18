namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность складского остатка продукта.
/// </summary>
public class Inventory
{
    private Inventory()
    {
    }

    public Inventory(Guid productId, int quantityInStock)
    {
        InventoryId = Guid.NewGuid();
        ProductId = productId;
        QuantityInStock = quantityInStock;
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// Уникальный идентификатор записи инвентаризации.
    /// </summary>
    public Guid InventoryId { get; private set; }

    /// <summary>
    /// Количество товара на складе.
    /// </summary>
    public int QuantityInStock { get; set; }

    /// <summary>
    /// Дата и время последнего обновления инвентаризации.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Идентификатор продукта (внешний ключ, уникальный — связь 1:1).
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Продукт, связанный с инвентаризацией.
    /// </summary>
    public Product Product { get; set; } = null!;
}

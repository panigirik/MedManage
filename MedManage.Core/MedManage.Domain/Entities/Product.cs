using MedManage.Domain.Enums;

namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность продукта.
/// </summary>
public class Product
{
    /// <summary>
    /// Уникальный идентификатор продукта.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Название продукта.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Тип продукта.
    /// </summary>
    public ProductType Type { get; set; }

    /// <summary>
    /// Цена продукта.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Дата истечения срока годности продукта.
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Навигационное свойство: информация об инвентаризации продукта.
    /// </summary>
    public Inventory Inventory { get; set; }
}
using MedManage.Application.DTOs;
using MedManage.Domain.Enums;

namespace MedManage.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с объявлениями.
/// </summary>
public interface IAnnouncementService
{
    /// <summary>
    /// Получить все объявления.
    /// </summary>
    Task<IEnumerable<AnnouncementDTO>> GetAllAnnouncementsAsync();

    /// <summary>
    /// Получить объявление по идентификатору.
    /// </summary>
    Task<AnnouncementDTO> GetAnnouncementByIdAsync(Guid announcementId);

    /// <summary>
    /// Получить объявления с пагинацией и фильтрацией.
    /// </summary>
    Task<IEnumerable<AnnouncementDTO>> GetAllAnnouncementsPaginatedAsync(
        int pageNumber, 
        int pageSize, 
        TypeOfSort sortBy, 
        string searchFilter, 
        ProductType productType,
        InventoryStatus statusInventory,
        int views);

    /// <summary>
    /// Создать новое объявление.
    /// </summary>
    Task<AnnouncementDTO> CreateNewAnnouncementAsync(AnnouncementDTO announcementRequest);

    /// <summary>
    /// Изменить содержимое объявления.
    /// </summary>
    Task ChangeAnnouncementContentAsync(Guid announcementId, string content);

    /// <summary>
    /// Удалить объявление.
    /// </summary>
    Task DeleteAnnouncementAsync(Guid announcementId);

    /// <summary>
    /// Получить объявления текущего пользователя.
    /// </summary>
    Task<IEnumerable<AnnouncementDTO>> GetMyAnnouncementsAsync();

    /// <summary>
    /// Получить объявления пользователя по идентификатору.
    /// </summary>
    Task<IEnumerable<AnnouncementDTO>> GetAnnouncementsByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить имя пользователя из токена.
    /// </summary>
    string GetUserNameFromToken();
}
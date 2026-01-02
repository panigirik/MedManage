using MedManage.Domain.Entities;
using MedManage.Domain.Enums;

namespace MedManage.Domain.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с объявлениями.
    /// </summary>
    public interface IAnnouncementRepository
    {
        /// <summary>
        /// Получение всех объявлений.
        /// </summary>
        /// <returns>Коллекция всех объявлений.</returns>
        Task<IEnumerable<Announcement>> GetAllAsync();

        /// <summary>
        /// Получение объявления по его уникальному идентификатору.
        /// </summary>
        /// <param name="announcementId">Уникальный идентификатор объявления.</param>
        /// <returns>Объявление с указанным идентификатором.</returns>
        Task<Announcement> GetByIdAsync(Guid announcementId);

        /// <summary>
        /// Получение списка объявлений с пагинацией.
        /// </summary>
        /// <param name="pageNumber">Номер страницы для пагинации.</param>
        /// <param name="pageSize">Количество элементов на странице.</param>
        /// <param name="searchFilter">Строка поиска по содержимому.</param>
        /// <param name="productType">Тип продукта для фильтрации.</param>
        /// <param name="inventoryStatus">Статус инвентаризации для фильтрации.</param>
        /// <param name="sortBy">Статус инвентаризации для фильтрации.</param>
        /// <returns>Коллекция объявлений с учетом пагинации.</returns>
        IQueryable<Announcement> GetPaginated(
            int pageNumber,
            int pageSize,
            TypeOfSort sortBy,
            string searchFilter,
            ProductType productType,
            InventoryStatus inventoryStatus);

        /// <summary>
        /// Создание нового объявления.
        /// </summary>
        /// <param name="announcement">Объявление для создания.</param>
        /// <returns>Задача, представляющая асинхронную операцию создания.</returns>
        Task CreateAsync(Announcement announcement);

        /// <summary>
        /// Обновление существующего объявления.
        /// </summary>
        /// <param name="announcement">Объявление с новыми данными для обновления.</param>
        /// <returns>Задача, представляющая асинхронную операцию обновления.</returns>
        Task UpdateAsync(Announcement announcement);

        /// <summary>
        /// Удаление объявления.
        /// </summary>
        /// <param name="announcement">Объявление для удаления.</param>
        /// <returns>Задача, представляющая асинхронную операцию удаления.</returns>
        Task DeleteAsync(Announcement announcement);

        /// <summary>
        /// Получение списка объявлений по имени автора.
        /// </summary>
        /// <param name="authorName">Имя автора для фильтрации.</param>
        /// <returns>Коллекция объявлений, написанных указанным автором.</returns>
        Task<IEnumerable<Announcement>> GetAnnouncementsByAuthorAsync(string authorName);

        /// <summary>
        /// Получение объявлений, созданных в указанную дату.
        /// </summary>
        /// <param name="date">Дата для фильтрации объявлений.</param>
        /// <returns>Коллекция объявлений, созданных в указанную дату.</returns>
        Task<IEnumerable<Announcement>> GetAnnouncementsByDateAsync(DateTime date);

        /// <summary>
        /// Поиск объявлений по содержимому.
        /// </summary>
        /// <param name="content">Содержимое для поиска.</param>
        /// <returns>Коллекция объявлений, которые содержат указанный текст.</returns>
        Task<IEnumerable<Announcement>> SearchAnnouncementsByContentAsync(string content);
    }
}

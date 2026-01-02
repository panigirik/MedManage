using Microsoft.EntityFrameworkCore;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore.Query;

namespace MedManage.Persistence.Repositories
{
    /// <summary>
    /// Репозиторий для работы с сущностью объявлений.
    /// Реализует интерфейс <see cref="IAnnouncementRepository"/>.
    /// </summary>
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly AnnouncementDbContext _context;

        /// <summary>
        /// Конструктор класса <see cref="AnnouncementRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public AnnouncementRepository(AnnouncementDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все объявления.
        /// </summary>
        /// <returns>Список всех объявлений.</returns>
        public async Task<IEnumerable<Announcement>> GetAllAsync()
        {
            return await _context.Announcements
                .Include(a => a.User) // Включаем пользователя, который создал объявление
                .OrderBy(a => a.CreatedAt) // Сортировка по дате создания
                .ToListAsync();
        }

        /// <summary>
        /// Получить объявление по идентификатору.
        /// </summary>
        /// <param name="announcementId">Идентификатор объявления.</param>
        /// <returns>Объект объявления.</returns>
        public async Task<Announcement> GetByIdAsync(Guid announcementId)
        {
            return await _context.Announcements
               .FirstOrDefaultAsync(a => a.AnnouncementId == announcementId); // Поиск по идентификатору
        }

        /// <summary>
        /// Получить объявления с пагинацией и фильтрацией.
        /// </summary>
        /// <param name="pageNumber">Номер страницы.</param>
        /// <param name="pageSize">Размер страницы.</param>
        /// <param name="searchFilter">Текст для поиска в заголовке и содержимом.</param>
        /// <param name="productType">Тип продукта для фильтрации.</param>
        /// <param name="inventoryStatus">Статус инвентаря для фильтрации.</param>
        /// <param name="sortBy">Статус инвентаря для фильтрации.</param>
        /// <returns>Отфильтрованные и отсортированные объявления.</returns>
        public IQueryable<Announcement> GetPaginated(
            int pageNumber,
            int pageSize,
            TypeOfSort sortBy,
            string searchFilter,
            ProductType productType,
            InventoryStatus inventoryStatus)
        {
            var announcements = _context.Announcements
                .Include(p => p.User) // Включаем пользователя
                .AsQueryable();

            // Фильтрация по типу продукта
            if (productType != ProductType.All)
            {
                announcements = announcements.Where(a => a.TypeProduct == productType);
            }

            // Фильтрация по статусу инвентаря
            if (inventoryStatus != InventoryStatus.All)
            {
                announcements = announcements.Where(a => a.StatusInventory == inventoryStatus);
            }

            // Фильтрация по тексту
            if (!string.IsNullOrWhiteSpace(searchFilter))
            {
                announcements = announcements.Where(a => a.Title.Contains(searchFilter) || a.Content.Contains(searchFilter));
            }

            // Сортировка по выбранному параметру
            switch (sortBy)
            {
                case TypeOfSort.ByCategory:
                    announcements = announcements.OrderByDescending(p => p.StatusInventory);
                    break;
                case TypeOfSort.ByDate:
                    announcements = announcements.OrderByDescending(p => p.CreatedAt);
                    break;
                default:
                    announcements = announcements.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            // Получаем список объявлений для пагинации
            
            var paginatedAnnouncements = announcements
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            foreach (var announcement in paginatedAnnouncements)
            {
                announcement.Views++;
                _context.Announcements.Update(announcement);
            }
            
            // Сохраняем изменения в базе данных
            _context.SaveChangesAsync();
            
            // Обновляем количество просмотров для всех записей
            var paginatedAnnouncementss = announcements
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsQueryable();
                 // Асинхронно загружаем нужные записи для отображения

            // Возвращаем обновленные объявления с пагинацией
            return paginatedAnnouncementss;
        }



        /// <summary>
        /// Создать новое объявление.
        /// </summary>
        /// <param name="announcement">Объявление для добавления.</param>
        /// <returns>Задача для асинхронного выполнения.</returns>
        public async Task CreateAsync(Announcement announcement)
        {
            await _context.Announcements.AddAsync(announcement); // Добавляем объявление в контекст
            await _context.SaveChangesAsync(); // Сохраняем изменения
        }

        /// <summary>
        /// Обновить существующее объявление.
        /// </summary>
        /// <param name="announcement">Обновленное объявление.</param>
        /// <returns>Задача для асинхронного выполнения.</returns>
        public async Task UpdateAsync(Announcement announcement)
        {
            _context.Announcements.Update(announcement); // Обновляем объявление в контексте
            await _context.SaveChangesAsync(); // Сохраняем изменения
        }

        /// <summary>
        /// Удалить объявление.
        /// </summary>
        /// <param name="announcement">Объявление для удаления.</param>
        /// <returns>Задача для асинхронного выполнения.</returns>
        public async Task DeleteAsync(Announcement announcement)
        {
            _context.Announcements.Remove(announcement); // Удаляем объявление из контекста
            await _context.SaveChangesAsync(); // Сохраняем изменения
        }

        /// <summary>
        /// Получить все объявления, созданные автором.
        /// </summary>
        /// <param name="authorName">Имя автора для фильтрации.</param>
        /// <returns>Список объявлений, созданных автором.</returns>
        public async Task<IEnumerable<Announcement>> GetAnnouncementsByAuthorAsync(string authorName)
        {
            return await _context.Announcements
                .Where(a => a.User.FullName.Contains(authorName)) // Фильтрация по имени автора
                .OrderByDescending(a => a.CreatedAt) // Сортировка по дате создания
                .ToListAsync();
        }

        /// <summary>
        /// Получить все объявления, созданные в определенную дату.
        /// </summary>
        /// <param name="date">Дата для фильтрации.</param>
        /// <returns>Список объявлений, созданных в указанную дату.</returns>
        public async Task<IEnumerable<Announcement>> GetAnnouncementsByDateAsync(DateTime date)
        {
            return await _context.Announcements
                .Where(a => a.CreatedAt.Date == date.Date) // Фильтрация по дате
                .OrderByDescending(a => a.CreatedAt) // Сортировка по дате создания
                .ToListAsync();
        }

        /// <summary>
        /// Найти объявления по содержимому.
        /// </summary>
        /// <param name="content">Текст для поиска в содержимом.</param>
        /// <returns>Список объявлений, содержащих текст.</returns>
        public async Task<IEnumerable<Announcement>> SearchAnnouncementsByContentAsync(string content)
        {
            return await _context.Announcements
                .Where(a => a.Content.Contains(content)) // Фильтрация по содержимому
                .ToListAsync();
        }
    }
}

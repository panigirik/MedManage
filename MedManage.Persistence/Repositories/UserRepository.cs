using MedManage.Domain.Entities;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories
{
    /// <summary>
    /// Репозиторий для работы с сущностью пользователей.
    /// Реализует интерфейс <see cref="IUserRepository"/>.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        /// <summary>
        /// Конструктор класса <see cref="UserRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных для пользователей.</param>
        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.User.FindAsync(userId);
        }
        
        /// <summary>
        /// Получить всех пользователей.
        /// </summary>
        /// <returns>Список всех пользователей.</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.User.ToListAsync(); // Возвращаем всех пользователей из базы данных
        }

        /// <summary>
        /// Получить пользователя по его идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Пользователь с указанным идентификатором.</returns>

        /// <summary>
        /// Получить всех пользователей (дублирующий метод).
        /// </summary>
        /// <returns>Список всех пользователей.</returns>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.User.ToListAsync(); // Возвращаем всех пользователей
        }

        /// <summary>
        /// Обновить информацию о пользователе.
        /// </summary>
        /// <param name="user">Пользователь с обновленной информацией.</param>
        /// <returns>Задача для асинхронного выполнения.</returns>
        public async Task UpdateAsync(User user)
        {
            _context.User.Update(user); // Обновляем пользователя в контексте
            await _context.SaveChangesAsync(); // Сохраняем изменения
        }

        /// <summary>
        /// Получить пользователя по его идентификатору (дублирующий метод).
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Пользователь с указанным идентификатором.</returns>
        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await _context.User.FindAsync(userId); // Ищем пользователя по идентификатору
        }

        /// <summary>
        /// Добавить нового пользователя.
        /// </summary>
        /// <param name="user">Пользователь, которого нужно добавить.</param>
        /// <returns>Задача для асинхронного выполнения.</returns>
        public async Task AddAsync(User user)
        {
            await _context.User.AddAsync(user); // Добавляем пользователя в контекст
            await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
        }
    }
}

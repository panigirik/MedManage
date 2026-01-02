using MedManage.Domain.Entities;

namespace MedManage.Domain.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с пользователями.
    /// </summary>
    public interface IUserRepository
    {
        
        Task<User> GetUserByIdAsync(Guid userId);
        
        /// <summary>
        /// Получение всех пользователей.
        /// </summary>
        /// <returns>Коллекция всех пользователей.</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();
        
        /// <summary>
        /// Получение пользователя по его уникальному идентификатору.
        /// </summary>
        /// <param name="userId">Уникальный идентификатор пользователя.</param>
        /// <returns>Пользователь с указанным идентификатором.</returns>
        Task<User> GetByIdAsync(Guid userId);
        
        /// <summary>
        /// Добавление нового пользователя.
        /// </summary>
        /// <param name="user">Пользователь для добавления.</param>
        /// <returns>Задача, представляющая асинхронную операцию добавления.</returns>
        Task AddAsync(User user);
        
        /// <summary>
        /// Обновление данных существующего пользователя.
        /// </summary>
        /// <param name="user">Пользователь с новыми данными для обновления.</param>
        /// <returns>Задача, представляющая асинхронную операцию обновления.</returns>
        Task UpdateAsync(User user);
    }
}
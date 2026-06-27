using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using AutoMapper.Extensions.ExpressionMapping;
using MedManage.Application.DTOs;
using MedManage.Application.Exceptions;
using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using MedManage.Domain.Interfaces;
using MedManage.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MedManage.Application.Services
{
    /// <summary>
    /// Сервис для управления объявлениями.
    /// </summary>
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IMapper _mapper;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Конструктор сервиса для управления объявлениями.
        /// </summary>
        public AnnouncementService(
            IMapper mapper,
            IAnnouncementRepository announcementRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository)
        {
            _mapper = mapper;
            _announcementRepository = announcementRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AnnouncementDTO>> GetAllAnnouncementsAsync()
        {
            var announcements = await _announcementRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AnnouncementDTO>>(announcements);
        }

        /// <inheritdoc />
        public async Task<AnnouncementDTO> GetAnnouncementByIdAsync(Guid announcementId)
        {
            var announcement = await _announcementRepository.GetByIdAsync(announcementId);
            if (announcement == null)
                throw new AnnouncementNotFoundException($"Объявление с ID {announcementId} не найдено.");
            return _mapper.Map<AnnouncementDTO>(announcement);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AnnouncementDTO>> GetAllAnnouncementsPaginatedAsync(
            int pageNumber,
            int pageSize,
            TypeOfSort sortBy,
            string searchFilter,
            ProductType productType,
            InventoryStatus statusInventory,
            int views)
        {
            var announcements = _announcementRepository.GetPaginated(
                pageNumber,
                pageSize,
                sortBy,
                searchFilter,
                productType,
                statusInventory);

            var projectedAnnouncements = announcements.ProjectTo<AnnouncementDTO>(_mapper.ConfigurationProvider);
            return await projectedAnnouncements.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<AnnouncementDTO> CreateNewAnnouncementAsync(AnnouncementDTO announcementRequest)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid UserId");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("Пользователь не найден.");

            announcementRequest.AnnouncementId = Guid.NewGuid();
            await _announcementRepository.CreateAsync(
                announcementRequest.Title,
                announcementRequest.Content,
                userId,
                announcementRequest.StatusInventory,
                announcementRequest.TypeProduct,
                user.OrganizationId,
                announcementRequest.ExpirationDate);

            return announcementRequest;
        }


        /// <inheritdoc />
        public async Task ChangeAnnouncementContentAsync(Guid announcementId, string content)
        {
            var announcement = await _announcementRepository.GetByIdAsync(announcementId);
            if (announcement == null)
                throw new AnnouncementNotFoundException($"Объявление с ID {announcementId} не найдено.");

            announcement.Content = content;
            announcement.UpdatedAt = DateTime.UtcNow;

            await _announcementRepository.UpdateAsync(announcement);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AnnouncementDTO>> GetMyAnnouncementsAsync()
        {
            var userId = GetCurrentUserId();
            return await GetAnnouncementsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<AnnouncementDTO>> GetAnnouncementsByUserIdAsync(Guid userId)
        {
            var announcements = await _announcementRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<AnnouncementDTO>>(announcements);
        }

        public async Task DeleteAnnouncementAsync(Guid announcementId)
        {
            var announcement = await _announcementRepository.GetByIdAsync(announcementId);
            if (announcement == null)
                throw new AnnouncementNotFoundException($"Объявление с ID {announcementId} не найдено.");

            await _announcementRepository.DeleteAsync(announcement);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user");
            return userId;
        }

        /// <inheritdoc />
        public string GetUserNameFromToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User?.Identity == null || !httpContext.User.Identity.IsAuthenticated)
                throw new InvalidOperationException("Пользователь не аутентифицирован.");

            var token = httpContext.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Токен авторизации отсутствует.");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "name");
            if (nameClaim == null)
                throw new InvalidOperationException("Поле 'name' отсутствует в токене.");

            return nameClaim.Value;
        }
    }
}

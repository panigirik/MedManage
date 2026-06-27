using System.Security.Claims;
using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace MedManage.Application.Services;

public class PurchaseRequestService : IPurchaseRequestService
{
    private readonly IPurchaseRequestRepository _purchaseRequestRepository;
    private readonly IAnnouncementRepository _announcementRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInAppNotificationRepository _inAppNotificationRepository;
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PurchaseRequestService> _logger;

    public PurchaseRequestService(
        IPurchaseRequestRepository purchaseRequestRepository,
        IAnnouncementRepository announcementRepository,
        IUserRepository userRepository,
        IInAppNotificationRepository inAppNotificationRepository,
        IAppDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ILogger<PurchaseRequestService> logger)
    {
        _purchaseRequestRepository = purchaseRequestRepository;
        _announcementRepository = announcementRepository;
        _userRepository = userRepository;
        _inAppNotificationRepository = inAppNotificationRepository;
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<PurchaseRequestDTO> CreateRequestAsync(CreatePurchaseRequestRequest request)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("CreateRequestAsync started. AnnouncementId={AnnouncementId}, BuyerUserId={BuyerUserId}",
            request.AnnouncementId, currentUserId);

        var announcement = await _announcementRepository.GetByIdAsync(request.AnnouncementId);
        if (announcement == null)
        {
            _logger.LogWarning("CreateRequestAsync: announcement {Id} not found", request.AnnouncementId);
            throw new InvalidOperationException("Объявление не найдено.");
        }
        _logger.LogInformation("CreateRequestAsync: announcement loaded, CreatedByUserId={CreatedBy}", announcement.CreatedByUserId);

        if (announcement.CreatedByUserId == currentUserId)
        {
            _logger.LogWarning("CreateRequestAsync: user {UserId} tried to request own announcement", currentUserId);
            throw new InvalidOperationException("Нельзя создать запрос на своё объявление.");
        }

        var currentUser = await _userRepository.GetByIdAsync(currentUserId);
        _logger.LogInformation("CreateRequestAsync: currentUser loaded, UserName={UserName}", currentUser.UserName);

        await using var transaction = await _context.BeginTransactionAsync();
        _logger.LogInformation("CreateRequestAsync: transaction started");

        try
        {
            var purchaseRequest = new PurchaseRequest(
                request.AnnouncementId,
                currentUserId,
                announcement.CreatedByUserId,
                request.Message);

            await _context.PurchaseRequests.AddAsync(purchaseRequest);
            _logger.LogInformation("CreateRequestAsync: PurchaseRequest added to context");

            var notification = new InAppNotification(
                announcement.CreatedByUserId,
                "Новый запрос на покупку",
                $"Пользователь {currentUser.UserName} хочет купить: {announcement.Title}",
                InAppNotificationType.PurchaseRequest,
                currentUserId,
                purchaseRequest.PurchaseRequestId);

            await _context.InAppNotifications.AddAsync(notification);
            _logger.LogInformation("CreateRequestAsync: notification added to context");

            await _context.SaveChangesAsync();
            _logger.LogInformation("CreateRequestAsync: SaveChangesAsync completed");

            await transaction.CommitAsync();
            _logger.LogInformation("CreateRequestAsync: transaction committed");

            var created = await _purchaseRequestRepository.GetByIdAsync(purchaseRequest.PurchaseRequestId);
            _logger.LogInformation("CreateRequestAsync: created purchase request {RequestId}", created?.PurchaseRequestId);
            return _mapper.Map<PurchaseRequestDTO>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateRequestAsync failed for AnnouncementId={AnnouncementId}", request.AnnouncementId);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseRequestDTO>> GetIncomingRequestsAsync()
    {
        var currentUserId = GetCurrentUserId();
        var requests = await _purchaseRequestRepository.GetBySellerAsync(currentUserId);
        return _mapper.Map<IEnumerable<PurchaseRequestDTO>>(requests);
    }

    public async Task<IEnumerable<PurchaseRequestDTO>> GetOutgoingRequestsAsync()
    {
        var currentUserId = GetCurrentUserId();
        var requests = await _purchaseRequestRepository.GetByBuyerAsync(currentUserId);
        return _mapper.Map<IEnumerable<PurchaseRequestDTO>>(requests);
    }

    public async Task AcceptRequestAsync(Guid requestId)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("AcceptRequestAsync: requestId={RequestId}, userId={UserId}", requestId, currentUserId);

        var request = await _purchaseRequestRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            _logger.LogWarning("AcceptRequestAsync: request {Id} not found", requestId);
            throw new InvalidOperationException("Запрос не найден.");
        }

        if (request.SellerUserId != currentUserId)
        {
            _logger.LogWarning("AcceptRequestAsync: user {UserId} is not seller of request {RequestId}", currentUserId, requestId);
            throw new UnauthorizedAccessException("Только продавец может принять запрос.");
        }

        if (request.Status != PurchaseRequestStatus.Pending)
        {
            _logger.LogWarning("AcceptRequestAsync: request {Id} already processed, status={Status}", requestId, request.Status);
            throw new InvalidOperationException("Запрос уже обработан.");
        }

        var announcementTitle = request.Announcement?.Title ?? "товар";
        var announcement = request.Announcement;

        await using var transaction = await _context.BeginTransactionAsync();
        _logger.LogInformation("AcceptRequestAsync: transaction started");

        try
        {
            request.Status = PurchaseRequestStatus.Accepted;
            request.UpdatedAt = DateTimeOffset.UtcNow;
            request.AnnouncementId = null;
            request.Announcement = null;

            if (announcement != null)
            {
                await _announcementRepository.DeleteAsync(announcement);
                _logger.LogInformation("AcceptRequestAsync: announcement {Id} deleted via repository", announcement.AnnouncementId);
            }

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);
            var notification = new InAppNotification(
                request.BuyerUserId,
                "Запрос на покупку принят",
                $"Пользователь {currentUser.UserName} принял ваш запрос на покупку: {announcementTitle}",
                InAppNotificationType.RequestAccepted,
                currentUserId,
                request.PurchaseRequestId);

            await _context.InAppNotifications.AddAsync(notification);
            _logger.LogInformation("AcceptRequestAsync: notification added");

            await _context.SaveChangesAsync();
            _logger.LogInformation("AcceptRequestAsync: SaveChangesAsync completed");

            await transaction.CommitAsync();
            _logger.LogInformation("AcceptRequestAsync: committed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AcceptRequestAsync failed for requestId={RequestId}", requestId);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task RejectRequestAsync(Guid requestId)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("RejectRequestAsync: requestId={RequestId}, userId={UserId}", requestId, currentUserId);

        var request = await _purchaseRequestRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            _logger.LogWarning("RejectRequestAsync: request {Id} not found", requestId);
            throw new InvalidOperationException("Запрос не найден.");
        }

        if (request.SellerUserId != currentUserId)
        {
            _logger.LogWarning("RejectRequestAsync: user {UserId} is not seller", currentUserId);
            throw new UnauthorizedAccessException("Только продавец может отклонить запрос.");
        }

        if (request.Status != PurchaseRequestStatus.Pending)
        {
            _logger.LogWarning("RejectRequestAsync: request {Id} already processed", requestId);
            throw new InvalidOperationException("Запрос уже обработан.");
        }

        await using var transaction = await _context.BeginTransactionAsync();
        _logger.LogInformation("RejectRequestAsync: transaction started");

        try
        {
            request.Status = PurchaseRequestStatus.Rejected;
            request.UpdatedAt = DateTimeOffset.UtcNow;

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);
            var announcementTitle = request.Announcement?.Title ?? "товар";
            var notification = new InAppNotification(
                request.BuyerUserId,
                "Запрос на покупку отклонён",
                $"Пользователь {currentUser.UserName} отклонил ваш запрос на покупку: {announcementTitle}",
                InAppNotificationType.RequestRejected,
                currentUserId,
                request.PurchaseRequestId);

            await _context.InAppNotifications.AddAsync(notification);
            _logger.LogInformation("RejectRequestAsync: notification added");

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("RejectRequestAsync: committed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RejectRequestAsync failed for requestId={RequestId}", requestId);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteRequestAsync(Guid requestId)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("DeleteRequestAsync: requestId={RequestId}, userId={UserId}", requestId, currentUserId);

        var request = await _purchaseRequestRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            _logger.LogWarning("DeleteRequestAsync: request {Id} not found", requestId);
            throw new InvalidOperationException("Запрос не найден.");
        }

        if (request.BuyerUserId != currentUserId && request.SellerUserId != currentUserId)
        {
            _logger.LogWarning("DeleteRequestAsync: user {UserId} is not participant", currentUserId);
            throw new UnauthorizedAccessException("Только участник запроса может его удалить.");
        }

        if (request.Status != PurchaseRequestStatus.Pending)
        {
            _logger.LogWarning("DeleteRequestAsync: request {Id} not pending", requestId);
            throw new InvalidOperationException("Нельзя удалить обработанный запрос.");
        }

        await using var transaction = await _context.BeginTransactionAsync();
        _logger.LogInformation("DeleteRequestAsync: transaction started");

        try
        {
            _context.PurchaseRequests.Remove(request);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("DeleteRequestAsync: request deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteRequestAsync failed for requestId={RequestId}", requestId);
            await transaction.RollbackAsync();
            throw;
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user");
        return userId;
    }
}

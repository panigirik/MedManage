using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Caching;
using MedManage.Persistence.Data;
using MedManage.Persistence.Transactions;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью объявлений.
/// </summary>
public class AnnouncementRepository : IAnnouncementRepository
{
    private const int RecentAnnouncementsLimit = 20;

    private readonly IAppDbContext _context;
    private readonly IInMemoryCache _cache; // только для массовой инвалидации в GetPaginated

    public AnnouncementRepository(IAppDbContext context, IInMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [Cache("RecentAnnouncements", ExpirationSeconds = 300)] // 5 минут
    public async Task<IEnumerable<Announcement>> GetAllAsync()
    {
        return await _context.Announcements
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .Take(RecentAnnouncementsLimit)
            .ToListAsync();
    }

    [Cache("ById:{announcementId}", ExpirationSeconds = 1800)] // 30 минут
    public async Task<Announcement> GetByIdAsync(Guid announcementId)
    {
        return await _context.Announcements
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AnnouncementId == announcementId);
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "ById:*")] // сбрасываем список и все кешированные объявления
    public IQueryable<Announcement> GetPaginated(
        int pageNumber,
        int pageSize,
        TypeOfSort sortBy,
        string searchFilter,
        ProductType productType,
        InventoryStatus inventoryStatus)
    {
        var announcements = _context.Announcements
            .Include(a => a.User)
            .AsQueryable();

        if (productType != ProductType.All)
            announcements = announcements.Where(a => a.TypeProduct == productType);

        if (inventoryStatus != InventoryStatus.All)
            announcements = announcements.Where(a => a.StatusInventory == inventoryStatus);

        if (!string.IsNullOrWhiteSpace(searchFilter))
            announcements = announcements.Where(a =>
                a.Title.Contains(searchFilter) || a.Content.Contains(searchFilter));

        announcements = sortBy switch
        {
            TypeOfSort.ByCategory => announcements.OrderByDescending(a => a.StatusInventory),
            TypeOfSort.ByDate => announcements.OrderByDescending(a => a.CreatedAt),
            _ => announcements.OrderByDescending(a => a.CreatedAt)
        };

        var paginatedAnnouncements = announcements
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        foreach (var announcement in paginatedAnnouncements)
        {
            announcement.Views++;
            _context.Announcements.Update(announcement);
        }

        _context.SaveChanges();

        // массовая инвалидация (дублирует атрибут, но атрибут сработает после метода)
        _cache.RemoveByPrefix("ById:");
        _cache.Remove(AnnouncementCacheKeys.RecentAnnouncements);

        return announcements
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "ById:*")]
    public async Task<Announcement> CreateAsync(
        string title,
        string content,
        Guid createdByUserId,
        InventoryStatus statusInventory,
        ProductType typeProduct,
        Guid? organizationId = null,
        DateTimeOffset? expirationDate = null)
    {
        var announcement = new Announcement(
            title,
            content,
            createdByUserId,
            statusInventory,
            typeProduct,
            organizationId,
            expirationDate);

        await _context.Announcements.AddAsync(announcement);
        await _context.SaveChangesAsync();

        var announcementWithUser = await _context.Announcements
            .Include(a => a.User)
            .FirstAsync(a => a.AnnouncementId == announcement.AnnouncementId);

        return announcementWithUser;
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "ById:*")]
    public async Task UpdateAsync(Announcement announcement)
    {
        _context.Announcements.Update(announcement);
        await _context.SaveChangesAsync();
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "ById:*")]
    public async Task DeleteAsync(Announcement announcement)
    {
        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Announcement>> GetAnnouncementsByAuthorAsync(string authorName)
    {
        return await _context.Announcements
            .Include(a => a.User)
            .Where(a => a.User.FullName.Contains(authorName))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> GetAnnouncementsByDateAsync(DateTime date)
    {
        return await _context.Announcements
            .Where(a => a.CreatedAt.Date == date.Date)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> SearchAnnouncementsByContentAsync(string content)
    {
        return await _context.Announcements
            .Where(a => a.Content.Contains(content))
            .ToListAsync();
    }
}
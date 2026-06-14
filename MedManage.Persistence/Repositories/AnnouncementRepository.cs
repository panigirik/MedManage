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
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    private readonly IAppDbContext _context;
    private readonly IInMemoryCache _cache;

    public AnnouncementRepository(IAppDbContext context, IInMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<Announcement>> GetAllAsync()
    {
        if (_cache.TryGet<List<Announcement>>(AnnouncementCacheKeys.RecentAnnouncements, out var cachedRecent)
            && cachedRecent is { Count: > 0 })
        {
            return cachedRecent;
        }

        return await _context.Announcements
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .Take(RecentAnnouncementsLimit)
            .ToListAsync();
    }

    public async Task<Announcement> GetByIdAsync(Guid announcementId)
    {
        if (_cache.TryGet<Announcement>(AnnouncementCacheKeys.ById(announcementId), out var cached)
            && cached is not null)
        {
            return cached;
        }

        var announcement = await _context.Announcements
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AnnouncementId == announcementId);

        if (announcement is not null)
        {
            CacheAnnouncement(announcement);
        }

        return announcement;
    }

    [Transactional]
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
        {
            announcements = announcements.Where(a => a.TypeProduct == productType);
        }

        if (inventoryStatus != InventoryStatus.All)
        {
            announcements = announcements.Where(a => a.StatusInventory == inventoryStatus);
        }

        if (!string.IsNullOrWhiteSpace(searchFilter))
        {
            announcements = announcements.Where(a =>
                a.Title.Contains(searchFilter) || a.Content.Contains(searchFilter));
        }

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
            _cache.Remove(AnnouncementCacheKeys.ById(announcement.AnnouncementId));
        }

        _context.SaveChanges();
        _cache.Remove(AnnouncementCacheKeys.RecentAnnouncements);

        return announcements
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    [Transactional]
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

        AddToRecentAnnouncementsCache(announcementWithUser);

        return announcementWithUser;
    }

    [Transactional]
    public async Task UpdateAsync(Announcement announcement)
    {
        _context.Announcements.Update(announcement);
        await _context.SaveChangesAsync();

        RemoveFromCache(announcement.AnnouncementId);
    }

    [Transactional]
    public async Task DeleteAsync(Announcement announcement)
    {
        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();

        RemoveFromCache(announcement.AnnouncementId);
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

    private void CacheAnnouncement(Announcement announcement)
    {
        _cache.Set(
            AnnouncementCacheKeys.ById(announcement.AnnouncementId),
            announcement,
            CacheExpiration);
    }

    private void AddToRecentAnnouncementsCache(Announcement announcement)
    {
        CacheAnnouncement(announcement);

        var recent = _cache.TryGet<List<Announcement>>(AnnouncementCacheKeys.RecentAnnouncements, out var cached)
            ? cached ?? new List<Announcement>()
            : new List<Announcement>();

        recent.RemoveAll(a => a.AnnouncementId == announcement.AnnouncementId);
        recent.Insert(0, announcement);

        if (recent.Count > RecentAnnouncementsLimit)
        {
            recent = recent.Take(RecentAnnouncementsLimit).ToList();
        }

        _cache.Set(AnnouncementCacheKeys.RecentAnnouncements, recent, CacheExpiration);
    }

    private void RemoveFromCache(Guid announcementId)
    {
        _cache.Remove(AnnouncementCacheKeys.ById(announcementId));
        _cache.Remove(AnnouncementCacheKeys.RecentAnnouncements);
    }
}

namespace MedManage.Persistence.Caching;

internal static class AnnouncementCacheKeys
{
    public const string RecentAnnouncements = "announcements:recent";

    public static string ById(Guid announcementId) => $"announcements:{announcementId}";
}

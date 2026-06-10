namespace ClinicManagement.API.Common;

/// <summary>
/// Thời gian theo giờ Việt Nam (UTC+7). Dùng nhất quán vì server cloud chạy UTC.
/// </summary>
public static class VietnamTime
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(7);

    public static DateTime Now => DateTime.UtcNow + Offset;
    public static DateOnly Today => DateOnly.FromDateTime(Now);
    public static int CurrentYear => Now.Year;
}

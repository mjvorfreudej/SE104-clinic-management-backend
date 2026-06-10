namespace ClinicManagement.API.Common;

/// <summary>
/// Mã code vai trò dùng cho phân quyền JWT ([Authorize(Roles = ...)]).
/// Tách khỏi tên hiển thị tiếng Việt để claim ổn định, không dấu.
/// </summary>
public static class VaiTroCode
{
    public const string Admin = "Admin";
    public const string BacSi = "BacSi";
    public const string TiepTan = "TiepTan";
    public const string KeToan = "KeToan";

    // Tổ hợp dùng cho [Authorize(Roles = ...)] (const concat hợp lệ ở compile-time)
    public const string TiepTan_Admin = TiepTan + "," + Admin;
    public const string BacSi_Admin = BacSi + "," + Admin;
    public const string KeToan_Admin = KeToan + "," + Admin;
}

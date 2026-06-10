namespace ClinicManagement.API.Models;

/// <summary>
/// Vai trò người dùng trong hệ thống (NFR – phân quyền).
/// 4 vai trò theo đặc tả: Tiếp Tân, Kế Toán, Bác Sĩ, Admin.
/// </summary>
public class VaiTro
{
    public int Id { get; set; }

    /// <summary>Mã code ổn định dùng cho phân quyền JWT: Admin, BacSi, TiepTan, KeToan.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Tên hiển thị tiếng Việt: "Tiếp Tân", "Kế Toán"...</summary>
    public string TenVaiTro { get; set; } = string.Empty;

    public ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}

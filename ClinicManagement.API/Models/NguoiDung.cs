namespace ClinicManagement.API.Models;

/// <summary>
/// Tài khoản người dùng đăng nhập hệ thống.
/// </summary>
public class NguoiDung
{
    public int Id { get; set; }

    /// <summary>Tên đăng nhập (dùng email theo giao diện Login của Frontend).</summary>
    public string TenDangNhap { get; set; } = string.Empty;

    /// <summary>Mật khẩu đã băm bằng BCrypt – KHÔNG bao giờ lưu plaintext.</summary>
    public string MatKhauHash { get; set; } = string.Empty;

    public string HoTen { get; set; } = string.Empty;

    public bool KichHoat { get; set; } = true;

    public int VaiTroId { get; set; }
    public VaiTro? VaiTro { get; set; }
}

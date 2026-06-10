using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.API.DTOs;

public class LoginRequest
{
    /// <summary>Email/tên đăng nhập (Frontend gửi field Email).</summary>
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string MatKhau { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;

    /// <summary>Mã code vai trò: Admin / BacSi / TiepTan / KeToan.</summary>
    public string VaiTroCode { get; set; } = string.Empty;

    /// <summary>Tên hiển thị vai trò tiếng Việt.</summary>
    public string VaiTro { get; set; } = string.Empty;
}

public class MeResponse
{
    public string TenDangNhap { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string VaiTroCode { get; set; } = string.Empty;
    public string VaiTro { get; set; } = string.Empty;
}

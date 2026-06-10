using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.API.DTOs;

public class LoginRequest
{
    /// <summary>Email/tên đăng nhập (Frontend gửi field Email).</summary>
    [Required]
    public string Email { get; set; } = string.Empty;

    /// <summary>Mật khẩu. Chấp nhận field "MatKhau" (chuẩn) hoặc "Password" (Frontend WPF đang bind).</summary>
    public string? MatKhau { get; set; }

    /// <summary>Alias của MatKhau – cho phép Frontend gửi "Password" mà vẫn đăng nhập được.</summary>
    public string? Password { get; set; }

    /// <summary>Mật khẩu hiệu lực: ưu tiên MatKhau, nếu trống thì lấy Password.</summary>
    public string GetMatKhau() => !string.IsNullOrEmpty(MatKhau) ? MatKhau : (Password ?? string.Empty);
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

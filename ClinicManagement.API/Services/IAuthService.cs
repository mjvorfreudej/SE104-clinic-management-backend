using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Services;

public interface IAuthService
{
    /// <summary>Trả về LoginResponse nếu hợp lệ; null nếu sai thông tin đăng nhập.</summary>
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}

using ClinicManagement.API.Data;
using ClinicManagement.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Services;

public class AuthService : IAuthService
{
    private readonly ClinicDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthService(ClinicDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var email = (request.Email ?? string.Empty).Trim().ToLower();

        var user = await _db.NguoiDungs
            .Include(u => u.VaiTro)
            .FirstOrDefaultAsync(u => u.TenDangNhap.ToLower() == email && u.KichHoat);

        if (user is null) return null;

        var matKhau = request.GetMatKhau();
        if (string.IsNullOrEmpty(matKhau)) return null;

        // Kiểm tra mật khẩu bằng BCrypt (chống lộ mật khẩu plaintext)
        if (!BCrypt.Net.BCrypt.Verify(matKhau, user.MatKhauHash))
            return null;

        var (token, expiresAt) = _tokenService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            TenDangNhap = user.TenDangNhap,
            HoTen = user.HoTen,
            VaiTroCode = user.VaiTro?.Code ?? string.Empty,
            VaiTro = user.VaiTro?.TenVaiTro ?? string.Empty
        };
    }
}

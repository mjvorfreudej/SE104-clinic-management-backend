using System.Security.Claims;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Đăng nhập, trả về JWT + vai trò.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result is null)
            return Unauthorized(new MessageResponse("Email hoặc mật khẩu không đúng."));
        return Ok(result);
    }

    /// <summary>Thông tin người dùng đang đăng nhập (kiểm tra token còn hiệu lực).</summary>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<MeResponse> Me()
    {
        return Ok(new MeResponse
        {
            TenDangNhap = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            HoTen = User.FindFirstValue("hoTen") ?? string.Empty,
            VaiTroCode = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
            VaiTro = User.FindFirstValue("vaiTroTen") ?? string.Empty
        });
    }
}

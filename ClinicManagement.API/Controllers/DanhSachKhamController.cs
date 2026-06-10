using System.Globalization;
using ClinicManagement.API.Common;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers;

/// <summary>YC1 – Lập danh sách khám bệnh.</summary>
[ApiController]
[Route("api/danhsachkham")]
[Authorize]
public class DanhSachKhamController : ControllerBase
{
    private readonly IDanhSachKhamService _service;

    public DanhSachKhamController(IDanhSachKhamService service) => _service = service;

    /// <summary>Danh sách khám hôm nay (mọi vai trò đã đăng nhập).</summary>
    [HttpGet("today")]
    public async Task<ActionResult<DanhSachKhamDto>> GetToday()
        => Ok(await _service.GetTodayAsync());

    /// <summary>Danh sách khám theo ngày (yyyy-MM-dd).</summary>
    [HttpGet]
    public async Task<ActionResult<DanhSachKhamDto>> GetByDate([FromQuery] string? ngay)
    {
        var date = VietnamTime.Today;
        if (!string.IsNullOrWhiteSpace(ngay) &&
            DateOnly.TryParse(ngay, CultureInfo.InvariantCulture, out var parsed))
        {
            date = parsed;
        }
        return Ok(await _service.GetByDateAsync(date));
    }

    /// <summary>Tiếp nhận bệnh nhân vào danh sách khám (Tiếp Tân / Admin).</summary>
    [HttpPost("tiepnhan")]
    [Authorize(Roles = VaiTroCode.TiepTan_Admin)]
    public async Task<ActionResult<ChiTietKhamItemDto>> TiepNhan([FromBody] DangKyKhamRequest request)
    {
        var result = await _service.TiepNhanAsync(request);
        return Ok(result);
    }
}

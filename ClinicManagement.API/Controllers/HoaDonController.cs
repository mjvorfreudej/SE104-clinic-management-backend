using ClinicManagement.API.Common;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers;

/// <summary>YC4 – Lập hóa đơn thanh toán (Tiếp Tân / Admin).</summary>
[ApiController]
[Route("api/hoadon")]
[Authorize]
public class HoaDonController : ControllerBase
{
    private readonly IHoaDonService _service;

    public HoaDonController(IHoaDonService service) => _service = service;

    /// <summary>Tính thử hóa đơn (không lưu) theo mã phiếu khám.</summary>
    [HttpGet("preview")]
    public async Task<ActionResult<HoaDonDto>> Preview([FromQuery] string maPhieuKham)
        => Ok(await _service.PreviewAsync(maPhieuKham));

    /// <summary>Lập & lưu hóa đơn.</summary>
    [HttpPost]
    [Authorize(Roles = VaiTroCode.TiepTan_Admin)]
    public async Task<ActionResult<HoaDonDto>> Create([FromBody] CreateHoaDonRequest request)
        => Ok(await _service.CreateAsync(request.MaPhieuKham));

    [HttpGet("{maHoaDon}")]
    public async Task<ActionResult<HoaDonDto>> GetByMa(string maHoaDon)
    {
        var dto = await _service.GetByMaAsync(maHoaDon);
        if (dto is null) return NotFound(new MessageResponse($"Không tìm thấy hóa đơn '{maHoaDon}'."));
        return Ok(dto);
    }
}

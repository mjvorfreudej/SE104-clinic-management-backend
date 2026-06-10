using ClinicManagement.API.DTOs;
using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers;

/// <summary>YC3 – Tra cứu bệnh nhân (mọi vai trò đã đăng nhập).</summary>
[ApiController]
[Route("api/tracuu")]
[Authorize]
public class TraCuuController : ControllerBase
{
    private readonly ITraCuuService _service;

    public TraCuuController(ITraCuuService service) => _service = service;

    /// <summary>Tra cứu theo họ tên / năm sinh / giới tính / ngày khám.</summary>
    [HttpGet("benhnhan")]
    public async Task<ActionResult<List<TraCuuBenhNhanResultDto>>> TraCuu(
        [FromQuery] string? hoTen,
        [FromQuery] int? namSinh,
        [FromQuery] string? gioiTinh,
        [FromQuery] DateTime? ngayKham)
        => Ok(await _service.TraCuuAsync(hoTen, namSinh, gioiTinh, ngayKham));

    /// <summary>Lịch sử khám của 1 bệnh nhân theo mã.</summary>
    [HttpGet("benhnhan/{maBenhNhan}/lichsu")]
    public async Task<ActionResult<List<LichSuKhamDto>>> LichSu(string maBenhNhan)
        => Ok(await _service.LichSuKhamAsync(maBenhNhan));
}

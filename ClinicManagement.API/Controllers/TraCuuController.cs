using ClinicManagement.API.Common;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers;

/// <summary>YC3 – Tra cứu bệnh nhân (tiếp tân + bác sĩ + admin theo đặc tả).</summary>
[ApiController]
[Route("api/tracuu")]
[Authorize(Roles = VaiTroCode.TiepTan_BacSi_Admin)]
public class TraCuuController : ControllerBase
{
    private readonly ITraCuuService _service;

    public TraCuuController(ITraCuuService service) => _service = service;

    /// <summary>Tra cứu theo họ tên / năm sinh / giới tính / SĐT / loại bệnh / ngày khám.</summary>
    [HttpGet("benhnhan")]
    public async Task<ActionResult<List<TraCuuBenhNhanResultDto>>> TraCuu(
        [FromQuery] string? hoTen,
        [FromQuery] int? namSinh,
        [FromQuery] string? gioiTinh,
        [FromQuery] DateTime? ngayKham,
        [FromQuery] string? soDienThoai,
        [FromQuery] int? loaiBenhId)
        => Ok(await _service.TraCuuAsync(hoTen, namSinh, gioiTinh, ngayKham, soDienThoai, loaiBenhId));

    /// <summary>Tìm hồ sơ bệnh nhân gần nhất theo SĐT (tự điền khi tiếp nhận). 404 nếu chưa có.</summary>
    [HttpGet("benhnhan/timsdt")]
    public async Task<ActionResult<BenhNhanInfoDto>> TimTheoSdt([FromQuery] string soDienThoai)
    {
        var bn = await _service.TimTheoSoDienThoaiAsync(soDienThoai);
        return bn is null ? NotFound(new MessageResponse("Không tìm thấy hồ sơ bệnh nhân với số điện thoại này.")) : Ok(bn);
    }

    /// <summary>Lịch sử khám của 1 bệnh nhân theo mã.</summary>
    [HttpGet("benhnhan/{maBenhNhan}/lichsu")]
    public async Task<ActionResult<List<LichSuKhamDto>>> LichSu(string maBenhNhan)
        => Ok(await _service.LichSuKhamAsync(maBenhNhan));
}

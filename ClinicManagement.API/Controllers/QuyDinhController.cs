using ClinicManagement.API.Common;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers;

/// <summary>YC6 – Thay đổi quy định (chỉ Admin).</summary>
[ApiController]
[Route("api/quydinh")]
[Authorize(Roles = VaiTroCode.Admin)]
public class QuyDinhController : ControllerBase
{
    private readonly IQuyDinhService _service;

    public QuyDinhController(IQuyDinhService service) => _service = service;

    // ----- Tham số QĐ1 / QĐ4 -----

    [HttpGet]
    public async Task<ActionResult<ThamSoDto>> GetThamSo() => Ok(await _service.GetThamSoAsync());

    [HttpPut]
    public async Task<ActionResult<ThamSoDto>> UpdateThamSo([FromBody] UpdateThamSoRequest request)
        => Ok(await _service.UpdateThamSoAsync(request));

    // ----- QĐ2: Loại bệnh -----

    [HttpPost("loaibenh")]
    public async Task<ActionResult<LoaiBenhDto>> AddLoaiBenh([FromBody] UpsertLoaiBenhRequest request)
        => Ok(await _service.AddLoaiBenhAsync(request));

    [HttpPut("loaibenh/{id:int}")]
    public async Task<ActionResult<LoaiBenhDto>> UpdateLoaiBenh(int id, [FromBody] UpsertLoaiBenhRequest request)
        => Ok(await _service.UpdateLoaiBenhAsync(id, request));

    [HttpDelete("loaibenh/{id:int}")]
    public async Task<IActionResult> DeleteLoaiBenh(int id)
    {
        await _service.DeleteLoaiBenhAsync(id);
        return Ok(new MessageResponse("Đã xóa loại bệnh."));
    }

    // ----- QĐ2 / QĐ4: Thuốc -----

    [HttpPost("thuoc")]
    public async Task<ActionResult<ThuocDto>> AddThuoc([FromBody] UpsertThuocRequest request)
        => Ok(await _service.AddThuocAsync(request));

    [HttpPut("thuoc/{id:int}")]
    public async Task<ActionResult<ThuocDto>> UpdateThuoc(int id, [FromBody] UpsertThuocRequest request)
        => Ok(await _service.UpdateThuocAsync(id, request));

    [HttpDelete("thuoc/{id:int}")]
    public async Task<IActionResult> DeleteThuoc(int id)
    {
        await _service.DeleteThuocAsync(id);
        return Ok(new MessageResponse("Đã xóa thuốc."));
    }

    // ----- QĐ2: Đơn vị tính -----

    [HttpPost("donvi")]
    public async Task<ActionResult<DonViDto>> AddDonVi([FromBody] UpsertDonViRequest request)
        => Ok(await _service.AddDonViAsync(request));

    [HttpPut("donvi/{id:int}")]
    public async Task<ActionResult<DonViDto>> UpdateDonVi(int id, [FromBody] UpsertDonViRequest request)
        => Ok(await _service.UpdateDonViAsync(id, request));

    [HttpDelete("donvi/{id:int}")]
    public async Task<IActionResult> DeleteDonVi(int id)
    {
        await _service.DeleteDonViAsync(id);
        return Ok(new MessageResponse("Đã xóa đơn vị tính."));
    }

    // ----- QĐ2: Cách dùng -----

    [HttpPost("cachdung")]
    public async Task<ActionResult<CachDungDto>> AddCachDung([FromBody] UpsertCachDungRequest request)
        => Ok(await _service.AddCachDungAsync(request));

    [HttpPut("cachdung/{id:int}")]
    public async Task<ActionResult<CachDungDto>> UpdateCachDung(int id, [FromBody] UpsertCachDungRequest request)
        => Ok(await _service.UpdateCachDungAsync(id, request));

    [HttpDelete("cachdung/{id:int}")]
    public async Task<IActionResult> DeleteCachDung(int id)
    {
        await _service.DeleteCachDungAsync(id);
        return Ok(new MessageResponse("Đã xóa cách dùng."));
    }
}

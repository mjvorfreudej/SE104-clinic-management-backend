using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers;

/// <summary>Danh mục dùng chung (loại bệnh, thuốc, đơn vị, cách dùng) – cần đăng nhập.</summary>
[ApiController]
[Route("api/danhmuc")]
[Authorize]
public class DanhMucController : ControllerBase
{
    private readonly IDanhMucService _service;

    public DanhMucController(IDanhMucService service) => _service = service;

    [HttpGet("loaibenh")]
    public async Task<IActionResult> GetLoaiBenh() => Ok(await _service.GetLoaiBenhAsync());

    [HttpGet("thuoc")]
    public async Task<IActionResult> GetThuoc() => Ok(await _service.GetThuocAsync());

    [HttpGet("donvi")]
    public async Task<IActionResult> GetDonVi() => Ok(await _service.GetDonViAsync());

    [HttpGet("cachdung")]
    public async Task<IActionResult> GetCachDung() => Ok(await _service.GetCachDungAsync());
}

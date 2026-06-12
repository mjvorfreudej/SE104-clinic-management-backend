using ClinicManagement.API.DTOs;
using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers;

/// <summary>
/// Đọc tham số hệ thống hiện hành (QĐ1 số BN tối đa, QĐ4 tiền khám) — cho MỌI vai trò đã đăng nhập.
/// Việc THAY ĐỔI quy định vẫn nằm ở QuyDinhController và chỉ Admin được phép.
/// </summary>
[ApiController]
[Route("api/thamso")]
[Authorize] // bất kỳ người dùng đã đăng nhập (Bác sĩ, Tiếp tân, Kế toán, Admin)
public class ThamSoController : ControllerBase
{
    private readonly IQuyDinhService _service;

    public ThamSoController(IQuyDinhService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ThamSoDto>> Get() => Ok(await _service.GetThamSoAsync());
}

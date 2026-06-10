using ClinicManagement.API.Common;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers;

/// <summary>YC2 – Lập phiếu khám bệnh.</summary>
[ApiController]
[Route("api/phieukham")]
[Authorize]
public class PhieuKhamController : ControllerBase
{
    private readonly IPhieuKhamService _service;

    public PhieuKhamController(IPhieuKhamService service) => _service = service;

    /// <summary>Lập phiếu khám (Bác Sĩ / Admin).</summary>
    [HttpPost]
    [Authorize(Roles = VaiTroCode.BacSi_Admin)]
    public async Task<ActionResult<PhieuKhamDto>> Create([FromBody] CreatePhieuKhamRequest request)
    {
        var result = await _service.CreateAsync(request);
        return Ok(result);
    }

    [HttpGet("{maPhieuKham}")]
    public async Task<ActionResult<PhieuKhamDto>> GetByMa(string maPhieuKham)
    {
        var dto = await _service.GetByMaAsync(maPhieuKham);
        if (dto is null) return NotFound(new MessageResponse($"Không tìm thấy phiếu khám '{maPhieuKham}'."));
        return Ok(dto);
    }
}

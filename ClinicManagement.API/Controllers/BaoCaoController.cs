using ClinicManagement.API.Common;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers;

/// <summary>YC5 – Lập báo cáo tháng (Kế Toán / Admin).</summary>
[ApiController]
[Route("api/baocao")]
[Authorize(Roles = VaiTroCode.KeToan_Admin)]
public class BaoCaoController : ControllerBase
{
    private readonly IBaoCaoService _service;

    public BaoCaoController(IBaoCaoService service) => _service = service;

    /// <summary>BM5.1 – Báo cáo doanh thu theo tháng.</summary>
    [HttpGet("doanhthu")]
    public async Task<ActionResult<BaoCaoDoanhThuDto>> DoanhThu([FromQuery] int thang, [FromQuery] int nam)
        => Ok(await _service.DoanhThuAsync(thang, nam));

    /// <summary>BM5.2 – Báo cáo sử dụng thuốc theo tháng.</summary>
    [HttpGet("sudungthuoc")]
    public async Task<ActionResult<BaoCaoSuDungThuocDto>> SuDungThuoc([FromQuery] int thang, [FromQuery] int nam)
        => Ok(await _service.SuDungThuocAsync(thang, nam));
}

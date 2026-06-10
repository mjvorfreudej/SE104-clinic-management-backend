using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Services;

public interface IBaoCaoService
{
    Task<BaoCaoDoanhThuDto> DoanhThuAsync(int thang, int nam);
    Task<BaoCaoSuDungThuocDto> SuDungThuocAsync(int thang, int nam);
}

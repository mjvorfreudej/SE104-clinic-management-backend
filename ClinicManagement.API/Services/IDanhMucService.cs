using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Services;

public interface IDanhMucService
{
    Task<List<LoaiBenhDto>> GetLoaiBenhAsync();
    Task<List<ThuocDto>> GetThuocAsync();
    Task<List<DonViDto>> GetDonViAsync();
    Task<List<CachDungDto>> GetCachDungAsync();
}

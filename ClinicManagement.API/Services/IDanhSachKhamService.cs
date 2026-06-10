using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Services;

public interface IDanhSachKhamService
{
    Task<DanhSachKhamDto> GetByDateAsync(DateOnly ngay);
    Task<DanhSachKhamDto> GetTodayAsync();
    Task<ChiTietKhamItemDto> TiepNhanAsync(DangKyKhamRequest request);
}

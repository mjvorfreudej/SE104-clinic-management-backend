using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Services;

public interface IQuyDinhService
{
    Task<ThamSoDto> GetThamSoAsync();
    Task<ThamSoDto> UpdateThamSoAsync(UpdateThamSoRequest request);

    // QĐ2 – quản lý danh mục loại bệnh
    Task<LoaiBenhDto> AddLoaiBenhAsync(UpsertLoaiBenhRequest request);
    Task<LoaiBenhDto> UpdateLoaiBenhAsync(int id, UpsertLoaiBenhRequest request);
    Task DeleteLoaiBenhAsync(int id);

    // QĐ2 / QĐ4 – quản lý danh mục thuốc (kèm đơn giá)
    Task<ThuocDto> AddThuocAsync(UpsertThuocRequest request);
    Task<ThuocDto> UpdateThuocAsync(int id, UpsertThuocRequest request);
    Task DeleteThuocAsync(int id);

    // QĐ2 – quản lý danh mục đơn vị tính
    Task<DonViDto> AddDonViAsync(UpsertDonViRequest request);
    Task<DonViDto> UpdateDonViAsync(int id, UpsertDonViRequest request);
    Task DeleteDonViAsync(int id);

    // QĐ2 – quản lý danh mục cách dùng
    Task<CachDungDto> AddCachDungAsync(UpsertCachDungRequest request);
    Task<CachDungDto> UpdateCachDungAsync(int id, UpsertCachDungRequest request);
    Task DeleteCachDungAsync(int id);
}

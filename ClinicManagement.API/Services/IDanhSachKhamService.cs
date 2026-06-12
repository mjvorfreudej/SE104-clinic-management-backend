using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Services;

public interface IDanhSachKhamService
{
    Task<DanhSachKhamDto> GetByDateAsync(DateOnly ngay);
    Task<DanhSachKhamDto> GetTodayAsync();
    Task<ChiTietKhamItemDto> TiepNhanAsync(DangKyKhamRequest request);

    /// <summary>Đánh dấu bệnh nhân đang được khám (Chờ khám -> Đang khám) cho ngày hôm nay.</summary>
    Task BatDauKhamAsync(string maBenhNhan);
}

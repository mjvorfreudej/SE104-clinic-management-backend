using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Services;

public interface IHoaDonService
{
    /// <summary>Tính thử hóa đơn (không lưu) từ phiếu khám.</summary>
    Task<HoaDonDto> PreviewAsync(string maPhieuKham);

    /// <summary>Lập & lưu hóa đơn (chặn xuất 2 lần – QĐ4).</summary>
    Task<HoaDonDto> CreateAsync(string maPhieuKham);

    Task<HoaDonDto?> GetByMaAsync(string maHoaDon);
}

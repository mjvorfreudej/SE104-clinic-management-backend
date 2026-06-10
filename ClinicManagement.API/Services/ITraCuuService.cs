using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Services;

public interface ITraCuuService
{
    Task<List<TraCuuBenhNhanResultDto>> TraCuuAsync(string? hoTen, int? namSinh, string? gioiTinh, DateTime? ngayKham);
    Task<List<LichSuKhamDto>> LichSuKhamAsync(string maBenhNhan);
}

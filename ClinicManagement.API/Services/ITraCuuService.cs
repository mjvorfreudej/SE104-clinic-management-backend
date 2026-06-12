using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Services;

public interface ITraCuuService
{
    Task<List<TraCuuBenhNhanResultDto>> TraCuuAsync(
        string? hoTen, int? namSinh, string? gioiTinh, DateTime? ngayKham,
        string? soDienThoai, int? loaiBenhId);

    Task<List<LichSuKhamDto>> LichSuKhamAsync(string maBenhNhan);

    /// <summary>Tìm hồ sơ bệnh nhân gần nhất theo số điện thoại (để tự điền khi tiếp nhận).</summary>
    Task<BenhNhanInfoDto?> TimTheoSoDienThoaiAsync(string soDienThoai);
}

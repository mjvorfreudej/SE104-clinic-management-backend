namespace ClinicManagement.API.Models;

/// <summary>
/// BM1 – Danh sách khám bệnh trong ngày (header, 1 bản ghi / 1 ngày khám).
/// </summary>
public class DanhSachKham
{
    public int Id { get; set; }

    /// <summary>Ngày khám (duy nhất mỗi ngày một danh sách).</summary>
    public DateOnly NgayKham { get; set; }

    /// <summary>Snapshot QĐ1 – số bệnh nhân tối đa áp dụng cho ngày này.</summary>
    public int SoBenhNhanToiDa { get; set; }

    public ICollection<ChiTietDanhSachKham> ChiTietDanhSachKhams { get; set; } = new List<ChiTietDanhSachKham>();
}

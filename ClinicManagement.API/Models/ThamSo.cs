namespace ClinicManagement.API.Models;

/// <summary>
/// Tham số / Quy định hệ thống (YC6). Bảng 1 dòng (singleton, Id = 1).
/// QĐ1: SoBenhNhanToiDaNgay; QĐ4: TienKham.
/// (QĐ2 – số loại bệnh/thuốc/đơn vị/cách dùng – được suy ra từ số bản ghi trong các bảng danh mục.)
/// </summary>
public class ThamSo
{
    public int Id { get; set; }

    /// <summary>QĐ1 – số bệnh nhân tối đa mỗi ngày (mặc định 40).</summary>
    public int SoBenhNhanToiDaNgay { get; set; } = 40;

    /// <summary>QĐ4 – tiền khám cố định (mặc định 30.000đ).</summary>
    public decimal TienKham { get; set; } = 30000m;
}

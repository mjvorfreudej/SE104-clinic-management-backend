namespace ClinicManagement.API.Models;

/// <summary>
/// Bệnh nhân (thực thể gốc dùng cho YC1, YC2, YC3, YC4).
/// </summary>
public class BenhNhan
{
    public int Id { get; set; }

    /// <summary>Mã bệnh nhân nghiệp vụ, ví dụ "BN0001".</summary>
    public string MaBenhNhan { get; set; } = string.Empty;

    public string HoTen { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public int NamSinh { get; set; }
    public string? DiaChi { get; set; }

    /// <summary>Số điện thoại bệnh nhân – dùng để tra cứu hồ sơ cũ (BM Tiếp nhận, BM3 Tra cứu).</summary>
    public string? SoDienThoai { get; set; }

    public ICollection<ChiTietDanhSachKham> ChiTietDanhSachKhams { get; set; } = new List<ChiTietDanhSachKham>();
    public ICollection<PhieuKham> PhieuKhams { get; set; } = new List<PhieuKham>();
}

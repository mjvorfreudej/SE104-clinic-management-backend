namespace ClinicManagement.API.Models;

/// <summary>Danh mục thuốc (QĐ2 – mặc định 30 loại; QĐ4 – mỗi loại có đơn giá riêng).</summary>
public class Thuoc
{
    public int Id { get; set; }
    public string MaThuoc { get; set; } = string.Empty;
    public string TenThuoc { get; set; } = string.Empty;
    public decimal DonGia { get; set; }

    public int DonViId { get; set; }
    public DonVi? DonVi { get; set; }

    public ICollection<ChiTietPhieuKham> ChiTietPhieuKhams { get; set; } = new List<ChiTietPhieuKham>();
}

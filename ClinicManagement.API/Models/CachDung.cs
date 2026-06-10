namespace ClinicManagement.API.Models;

/// <summary>Danh mục cách dùng thuốc (QĐ2 – mặc định 4 cách dùng).</summary>
public class CachDung
{
    public int Id { get; set; }
    public string MaCachDung { get; set; } = string.Empty;
    public string MoTaCachDung { get; set; } = string.Empty;

    public ICollection<ChiTietPhieuKham> ChiTietPhieuKhams { get; set; } = new List<ChiTietPhieuKham>();
}

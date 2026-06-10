namespace ClinicManagement.API.Models;

/// <summary>Danh mục loại bệnh (QĐ2 – mặc định 5 loại).</summary>
public class LoaiBenh
{
    public int Id { get; set; }
    public string MaLoaiBenh { get; set; } = string.Empty;
    public string TenLoaiBenh { get; set; } = string.Empty;

    public ICollection<PhieuKham> PhieuKhams { get; set; } = new List<PhieuKham>();
}

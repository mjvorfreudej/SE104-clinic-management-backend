namespace ClinicManagement.API.Models;

/// <summary>Danh mục đơn vị tính thuốc (QĐ2 – mặc định: Viên, Chai).</summary>
public class DonVi
{
    public int Id { get; set; }
    public string MaDonVi { get; set; } = string.Empty;
    public string TenDonVi { get; set; } = string.Empty;

    public ICollection<Thuoc> Thuocs { get; set; } = new List<Thuoc>();
}

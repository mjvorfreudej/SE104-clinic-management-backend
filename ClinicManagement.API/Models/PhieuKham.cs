namespace ClinicManagement.API.Models;

/// <summary>
/// BM2 – Phiếu khám bệnh (do Bác Sĩ lập). Đầu vào cho YC4 (hóa đơn) và YC5 (báo cáo).
/// </summary>
public class PhieuKham
{
    public int Id { get; set; }

    /// <summary>Mã phiếu khám nghiệp vụ, ví dụ "PK0001".</summary>
    public string MaPhieuKham { get; set; } = string.Empty;

    public int BenhNhanId { get; set; }
    public BenhNhan? BenhNhan { get; set; }

    public DateOnly NgayKham { get; set; }

    public string? TrieuChung { get; set; }

    public int? LoaiBenhId { get; set; }
    public LoaiBenh? LoaiBenh { get; set; }

    public ICollection<ChiTietPhieuKham> ChiTietPhieuKhams { get; set; } = new List<ChiTietPhieuKham>();

    public HoaDon? HoaDon { get; set; }
}

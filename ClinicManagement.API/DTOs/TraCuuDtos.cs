namespace ClinicManagement.API.DTOs;

/// <summary>BM3 – Một dòng kết quả tra cứu bệnh nhân.</summary>
public class TraCuuBenhNhanResultDto
{
    public int STT { get; set; }
    public string MaBenhNhan { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public int NamSinh { get; set; }
    public DateTime? NgayKham { get; set; }
    public string? TenLoaiBenh { get; set; }
    public string? TrieuChung { get; set; }
}

/// <summary>Lịch sử khám của 1 bệnh nhân.</summary>
public class LichSuKhamDto
{
    public string MaPhieuKham { get; set; } = string.Empty;
    public DateTime NgayKham { get; set; }
    public string? TenLoaiBenh { get; set; }
    public string? TrieuChung { get; set; }
    public List<ChiTietToaThuocDto> ToaThuoc { get; set; } = new();
}

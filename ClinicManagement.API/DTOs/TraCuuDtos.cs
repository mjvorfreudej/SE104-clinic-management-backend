namespace ClinicManagement.API.DTOs;

/// <summary>BM3 – Một dòng kết quả tra cứu bệnh nhân.</summary>
public class TraCuuBenhNhanResultDto
{
    public int STT { get; set; }
    public string MaBenhNhan { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public int NamSinh { get; set; }
    public string? SoDienThoai { get; set; }
    public DateTime? NgayKham { get; set; }
    public string? TenLoaiBenh { get; set; }
    public string? TrieuChung { get; set; }
}

/// <summary>Thông tin hồ sơ bệnh nhân (dùng để tự điền khi tiếp nhận – tra theo SĐT).</summary>
public class BenhNhanInfoDto
{
    public string MaBenhNhan { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public int NamSinh { get; set; }
    public string? DiaChi { get; set; }
    public string? SoDienThoai { get; set; }
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

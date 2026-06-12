using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.API.DTOs;

/// <summary>YC2 – Yêu cầu lập phiếu khám bệnh (BM2).</summary>
public class CreatePhieuKhamRequest
{
    [Required]
    public string MaBenhNhan { get; set; } = string.Empty;

    public DateTime NgayKham { get; set; }

    public string? TrieuChung { get; set; }

    /// <summary>Mã loại bệnh từ danh mục (LB1..LB5).</summary>
    public string? MaLoaiBenh { get; set; }

    public List<ChiTietToaThuocRequest> ToaThuoc { get; set; } = new();
}

public class ChiTietToaThuocRequest
{
    [Required]
    public string MaThuoc { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng thuốc phải là số nguyên dương")]
    public int SoLuong { get; set; }

    [Required]
    public string MaCachDung { get; set; } = string.Empty;
}

public class PhieuKhamDto
{
    public string MaPhieuKham { get; set; } = string.Empty;
    public string MaBenhNhan { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public DateTime NgayKham { get; set; }
    public string? TrieuChung { get; set; }
    public string? MaLoaiBenh { get; set; }
    public string? TenLoaiBenh { get; set; }
    public bool DaLapHoaDon { get; set; }
    public List<ChiTietToaThuocDto> ToaThuoc { get; set; } = new();
}

public class ChiTietToaThuocDto
{
    public string MaThuoc { get; set; } = string.Empty;
    public string TenThuoc { get; set; } = string.Empty;
    public string TenDonVi { get; set; } = string.Empty;
    public int SoLuong { get; set; }
    public string MaCachDung { get; set; } = string.Empty;
    public string MoTaCachDung { get; set; } = string.Empty;
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}

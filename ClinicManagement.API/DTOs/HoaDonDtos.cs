using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.API.DTOs;

/// <summary>YC4 – Yêu cầu lập hóa đơn từ một phiếu khám.</summary>
public class CreateHoaDonRequest
{
    [Required]
    public string MaPhieuKham { get; set; } = string.Empty;
}

/// <summary>BM4 – Hóa đơn thanh toán (dùng cho cả preview và bản đã lưu).</summary>
public class HoaDonDto
{
    public string? MaHoaDon { get; set; }
    public string MaPhieuKham { get; set; } = string.Empty;
    public string MaBenhNhan { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public DateTime NgayKham { get; set; }
    public decimal TienKham { get; set; }
    public decimal TienThuoc { get; set; }
    public decimal TongTien { get; set; }
    public bool DaThanhToan { get; set; }
    public List<ChiTietToaThuocDto> ChiTietThuoc { get; set; } = new();
}

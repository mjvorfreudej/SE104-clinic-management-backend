namespace ClinicManagement.API.Models;

/// <summary>
/// BM4 – Hóa đơn thanh toán. Mỗi phiếu khám chỉ xuất 1 hóa đơn (tránh thu tiền 2 lần – QĐ4).
/// </summary>
public class HoaDon
{
    public int Id { get; set; }

    public string MaHoaDon { get; set; } = string.Empty;

    public int PhieuKhamId { get; set; }
    public PhieuKham? PhieuKham { get; set; }

    public decimal TienKham { get; set; }
    public decimal TienThuoc { get; set; }
    public decimal TongTien { get; set; }

    public DateOnly NgayLap { get; set; }

    public bool DaThanhToan { get; set; } = true;
}

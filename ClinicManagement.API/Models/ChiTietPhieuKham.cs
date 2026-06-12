namespace ClinicManagement.API.Models;

/// <summary>
/// Một dòng toa thuốc trong phiếu khám.
/// Theo Thiết kế dữ liệu mục 2.4 &amp; mục 3 (tối ưu hóa mô hình về mặt thời gian):
/// các thông tin TenThuoc, DonGia, TenDonVi, TenCachDung được SNAPSHOT trực tiếp tại
/// thời điểm kê đơn nhằm giảm số phép JOIN khi truy vấn/báo cáo và giữ nguyên dữ liệu
/// lịch sử (không bị sai lệch khi giá/đơn vị/cách dùng thay đổi về sau).
/// </summary>
public class ChiTietPhieuKham
{
    public int Id { get; set; }

    public int PhieuKhamId { get; set; }
    public PhieuKham? PhieuKham { get; set; }

    public int ThuocId { get; set; }
    public Thuoc? Thuoc { get; set; }

    /// <summary>Tên thuốc sao chép tại thời điểm kê (snapshot lịch sử).</summary>
    public string TenThuoc { get; set; } = string.Empty;

    /// <summary>Đơn vị tính sao chép tại thời điểm kê (snapshot lịch sử – giảm JOIN sang DonVi).</summary>
    public string TenDonVi { get; set; } = string.Empty;

    public int SoLuong { get; set; }

    public int CachDungId { get; set; }
    public CachDung? CachDung { get; set; }

    /// <summary>Cách dùng sao chép tại thời điểm kê (snapshot lịch sử – giảm JOIN sang CachDung).</summary>
    public string TenCachDung { get; set; } = string.Empty;

    /// <summary>Đơn giá sao chép tại thời điểm kê (snapshot lịch sử).</summary>
    public decimal DonGia { get; set; }

    public decimal ThanhTien => SoLuong * DonGia;
}

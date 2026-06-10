namespace ClinicManagement.API.Models;

/// <summary>
/// Một dòng toa thuốc trong phiếu khám.
/// TenThuoc và DonGia được SNAPSHOT tại thời điểm kê đơn (theo Thiết kế dữ liệu mục 2.4)
/// để không bị sai lệch lịch sử khi giá thuốc thay đổi về sau.
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

    public int SoLuong { get; set; }

    public int CachDungId { get; set; }
    public CachDung? CachDung { get; set; }

    /// <summary>Đơn giá sao chép tại thời điểm kê (snapshot lịch sử).</summary>
    public decimal DonGia { get; set; }

    public decimal ThanhTien => SoLuong * DonGia;
}

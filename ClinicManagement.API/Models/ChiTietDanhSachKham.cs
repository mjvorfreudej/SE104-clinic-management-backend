namespace ClinicManagement.API.Models;

/// <summary>
/// Chi tiết một dòng trong danh sách khám của ngày (1 bệnh nhân đăng ký khám).
/// </summary>
public class ChiTietDanhSachKham
{
    public int Id { get; set; }

    public int DanhSachKhamId { get; set; }
    public DanhSachKham? DanhSachKham { get; set; }

    public int BenhNhanId { get; set; }
    public BenhNhan? BenhNhan { get; set; }

    /// <summary>Số thứ tự khám trong ngày (server cấp phát).</summary>
    public int STT { get; set; }

    /// <summary>Trạng thái: "Chờ khám", "Đã khám".</summary>
    public string TrangThai { get; set; } = "Chờ khám";
}

using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.API.DTOs;

/// <summary>BM1 – Gói dữ liệu danh sách khám trong ngày (khớp DTO của Frontend).</summary>
public class DanhSachKhamDto
{
    public int Id { get; set; }
    public DateTime NgayKham { get; set; }
    public int SoBenhNhanToiDaNgay { get; set; }
    public decimal TongDoanhThuNgay { get; set; }
    public List<ChiTietKhamItemDto> ChiTietDanhSach { get; set; } = new();
}

public class ChiTietKhamItemDto
{
    public int STT { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string MaBenhNhan { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public int NamSinh { get; set; }
    public string? DiaChi { get; set; }
    public string? SoDienThoai { get; set; }

    /// <summary>Mã phiếu khám (null nếu bệnh nhân chưa được lập phiếu).</summary>
    public string? MaPhieuKham { get; set; }
}

/// <summary>YC1 – Tiếp nhận bệnh nhân vào danh sách khám.</summary>
public class DangKyKhamRequest
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giới tính không được để trống")]
    public string GioiTinh { get; set; } = string.Empty;

    [Range(1900, 9999, ErrorMessage = "Năm sinh không hợp lệ")]
    public int NamSinh { get; set; }

    public string? DiaChi { get; set; }

    /// <summary>Số điện thoại; dùng để nhận diện/tra cứu hồ sơ bệnh nhân cũ.</summary>
    public string? SoDienThoai { get; set; }

    /// <summary>Ngày khám; nếu để trống/MinValue sẽ mặc định là hôm nay.</summary>
    public DateTime NgayKham { get; set; }
}

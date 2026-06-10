using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.API.DTOs;

public class LoaiBenhDto
{
    public int Id { get; set; }
    public string MaLoaiBenh { get; set; } = string.Empty;
    public string TenLoaiBenh { get; set; } = string.Empty;
}

public class DonViDto
{
    public int Id { get; set; }
    public string MaDonVi { get; set; } = string.Empty;
    public string TenDonVi { get; set; } = string.Empty;
}

public class CachDungDto
{
    public int Id { get; set; }
    public string MaCachDung { get; set; } = string.Empty;
    public string MoTaCachDung { get; set; } = string.Empty;
}

public class ThuocDto
{
    public int Id { get; set; }
    public string MaThuoc { get; set; } = string.Empty;
    public string TenThuoc { get; set; } = string.Empty;
    public decimal DonGia { get; set; }
    public string MaDonVi { get; set; } = string.Empty;
    public string TenDonVi { get; set; } = string.Empty;
}

// ---- YC6: yêu cầu thêm/sửa danh mục ----

public class UpsertLoaiBenhRequest
{
    [Required] public string TenLoaiBenh { get; set; } = string.Empty;
    public string? MaLoaiBenh { get; set; }
}

public class UpsertThuocRequest
{
    [Required] public string TenThuoc { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải >= 0")]
    public decimal DonGia { get; set; }

    [Required] public string MaDonVi { get; set; } = string.Empty;
    public string? MaThuoc { get; set; }
}

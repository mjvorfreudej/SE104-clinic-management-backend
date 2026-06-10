using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.API.DTOs;

/// <summary>YC6 – Tham số / quy định hệ thống (QĐ1, QĐ4) + thống kê danh mục (QĐ2).</summary>
public class ThamSoDto
{
    public int SoBenhNhanToiDaNgay { get; set; }
    public decimal TienKham { get; set; }

    // Thống kê QĐ2 (chỉ đọc)
    public int SoLoaiBenh { get; set; }
    public int SoLoaiThuoc { get; set; }
    public int SoDonVi { get; set; }
    public int SoCachDung { get; set; }
}

public class UpdateThamSoRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Số bệnh nhân tối đa phải là số dương")]
    public int SoBenhNhanToiDaNgay { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Tiền khám phải >= 0")]
    public decimal TienKham { get; set; }
}

/// <summary>Phản hồi lỗi đơn giản (Frontend chỉ cần đọc message).</summary>
public class MessageResponse
{
    public string Message { get; set; } = string.Empty;
    public MessageResponse() { }
    public MessageResponse(string message) => Message = message;
}

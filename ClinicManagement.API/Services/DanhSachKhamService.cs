using ClinicManagement.API.Common;
using ClinicManagement.API.Data;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Services;

/// <summary>YC1 – Lập danh sách khám bệnh (BM1, QĐ1: tối đa 40 BN/ngày).</summary>
public class DanhSachKhamService : IDanhSachKhamService
{
    private readonly ClinicDbContext _db;

    public DanhSachKhamService(ClinicDbContext db) => _db = db;

    public Task<DanhSachKhamDto> GetTodayAsync() => GetByDateAsync(VietnamTime.Today);

    public async Task<DanhSachKhamDto> GetByDateAsync(DateOnly ngay)
    {
        var thamSo = await _db.ThamSos.AsNoTracking().FirstOrDefaultAsync();
        var maxMacDinh = thamSo?.SoBenhNhanToiDaNgay ?? 40;

        var dsk = await _db.DanhSachKhams
            .AsNoTracking()
            .Include(d => d.ChiTietDanhSachKhams).ThenInclude(c => c.BenhNhan)
            .FirstOrDefaultAsync(d => d.NgayKham == ngay);

        // Bản đồ BenhNhanId -> MaPhieuKham cho ngày này (để biết ai đã lập phiếu).
        // Gom nhóm trong bộ nhớ để tránh truy vấn GroupBy().First() khó dịch sang SQL.
        var phieuList = await _db.PhieuKhams
            .AsNoTracking()
            .Where(p => p.NgayKham == ngay)
            .Select(p => new { p.BenhNhanId, p.MaPhieuKham })
            .ToListAsync();
        var phieuTheoBn = phieuList
            .GroupBy(x => x.BenhNhanId)
            .ToDictionary(g => g.Key, g => g.First().MaPhieuKham);

        var doanhThu = await _db.HoaDons
            .AsNoTracking()
            .Where(h => h.NgayLap == ngay)
            .SumAsync(h => (decimal?)h.TongTien) ?? 0m;

        var dto = new DanhSachKhamDto
        {
            Id = dsk?.Id ?? 0,
            NgayKham = ngay.ToDateTime(TimeOnly.MinValue),
            // Luôn lấy giới hạn theo quy định HIỆN HÀNH (QĐ1) để khi Admin đổi quy định là mọi màn hình
            // cập nhật ngay, không bị kẹt ở giá trị đã "đóng băng" lúc tạo danh sách.
            SoBenhNhanToiDaNgay = maxMacDinh,
            TongDoanhThuNgay = doanhThu
        };

        if (dsk is not null)
        {
            foreach (var ct in dsk.ChiTietDanhSachKhams.OrderBy(c => c.STT))
            {
                dto.ChiTietDanhSach.Add(new ChiTietKhamItemDto
                {
                    STT = ct.STT,
                    TrangThai = ct.TrangThai,
                    MaBenhNhan = ct.BenhNhan?.MaBenhNhan ?? string.Empty,
                    HoTen = ct.BenhNhan?.HoTen ?? string.Empty,
                    GioiTinh = ct.BenhNhan?.GioiTinh ?? string.Empty,
                    NamSinh = ct.BenhNhan?.NamSinh ?? 0,
                    DiaChi = ct.BenhNhan?.DiaChi,
                    MaPhieuKham = phieuTheoBn.TryGetValue(ct.BenhNhanId, out var ma) ? ma : null
                });
            }
        }

        return dto;
    }

    public async Task<ChiTietKhamItemDto> TiepNhanAsync(DangKyKhamRequest request)
    {
        // --- Validate dữ liệu (Requires của YC1) ---
        if (string.IsNullOrWhiteSpace(request.HoTen))
            throw new DomainException("Họ tên không được để trống.");
        if (string.IsNullOrWhiteSpace(request.GioiTinh))
            throw new DomainException("Giới tính không được để trống.");
        if (request.NamSinh < 1900 || request.NamSinh > VietnamTime.CurrentYear)
            throw new DomainException($"Năm sinh phải từ 1900 đến {VietnamTime.CurrentYear}.");

        var ngay = request.NgayKham == default
            ? VietnamTime.Today
            : DateOnly.FromDateTime(request.NgayKham);

        // --- Giới hạn QĐ1 theo quy định HIỆN HÀNH (không dùng giá trị đã đóng băng) ---
        var thamSo = await _db.ThamSos.FirstOrDefaultAsync();
        var gioiHanNgay = thamSo?.SoBenhNhanToiDaNgay ?? 40;

        // --- Lấy hoặc tạo danh sách khám của ngày ---
        var dsk = await _db.DanhSachKhams.FirstOrDefaultAsync(d => d.NgayKham == ngay);
        if (dsk is null)
        {
            dsk = new DanhSachKham
            {
                NgayKham = ngay,
                SoBenhNhanToiDa = gioiHanNgay
            };
            _db.DanhSachKhams.Add(dsk);
            await _db.SaveChangesAsync(); // cần Id của header
        }

        // --- Kiểm tra QĐ1: giới hạn số bệnh nhân/ngày ---
        var soHienTai = await _db.ChiTietDanhSachKhams.CountAsync(c => c.DanhSachKhamId == dsk.Id);
        if (soHienTai >= gioiHanNgay)
            throw new DomainException($"Đã đủ {gioiHanNgay} bệnh nhân trong ngày, không thể tiếp nhận thêm.");

        // --- Tạo bệnh nhân mới (taoBenhNhan) ---
        var existingCodes = await _db.BenhNhans.Select(b => b.MaBenhNhan).ToListAsync();
        var benhNhan = new BenhNhan
        {
            MaBenhNhan = MaGenerator.Next("BN", existingCodes),
            HoTen = request.HoTen.Trim(),
            GioiTinh = request.GioiTinh.Trim(),
            NamSinh = request.NamSinh,
            DiaChi = request.DiaChi?.Trim()
        };
        _db.BenhNhans.Add(benhNhan);
        await _db.SaveChangesAsync();

        // --- Thêm vào danh sách khám (themBenhNhan) với STT tự cấp ---
        var chiTiet = new ChiTietDanhSachKham
        {
            DanhSachKhamId = dsk.Id,
            BenhNhanId = benhNhan.Id,
            STT = soHienTai + 1,
            TrangThai = "Chờ khám"
        };
        _db.ChiTietDanhSachKhams.Add(chiTiet);
        await _db.SaveChangesAsync();

        return new ChiTietKhamItemDto
        {
            STT = chiTiet.STT,
            TrangThai = chiTiet.TrangThai,
            MaBenhNhan = benhNhan.MaBenhNhan,
            HoTen = benhNhan.HoTen,
            GioiTinh = benhNhan.GioiTinh,
            NamSinh = benhNhan.NamSinh,
            DiaChi = benhNhan.DiaChi,
            MaPhieuKham = null
        };
    }
}

using ClinicManagement.API.Common;
using ClinicManagement.API.Data;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Services;

/// <summary>YC4 – Lập hóa đơn thanh toán (BM4, QĐ4).</summary>
public class HoaDonService : IHoaDonService
{
    private readonly ClinicDbContext _db;

    public HoaDonService(ClinicDbContext db) => _db = db;

    private async Task<PhieuKham> LayPhieuKhamAsync(string maPhieuKham)
    {
        return await _db.PhieuKhams
            .Include(p => p.BenhNhan)
            .Include(p => p.HoaDon)
            .Include(p => p.ChiTietPhieuKhams).ThenInclude(c => c.CachDung)
            .Include(p => p.ChiTietPhieuKhams).ThenInclude(c => c.Thuoc)
            .FirstOrDefaultAsync(p => p.MaPhieuKham == maPhieuKham)
            ?? throw new DomainException($"Không tìm thấy phiếu khám '{maPhieuKham}'.");
    }

    public async Task<HoaDonDto> PreviewAsync(string maPhieuKham)
    {
        var phieu = await LayPhieuKhamAsync(maPhieuKham);
        var thamSo = await _db.ThamSos.AsNoTracking().FirstOrDefaultAsync();
        var tienKham = thamSo?.TienKham ?? 30000m;
        return BuildDto(phieu, tienKham, phieu.HoaDon);
    }

    public async Task<HoaDonDto> CreateAsync(string maPhieuKham)
    {
        var phieu = await LayPhieuKhamAsync(maPhieuKham);

        // QĐ4: không xuất hóa đơn 2 lần cho cùng phiếu khám
        if (phieu.HoaDon is not null)
            throw new DomainException($"Phiếu khám '{maPhieuKham}' đã được lập hóa đơn ({phieu.HoaDon.MaHoaDon}).");

        var thamSo = await _db.ThamSos.FirstOrDefaultAsync();
        var tienKham = thamSo?.TienKham ?? 30000m;
        var tienThuoc = phieu.ChiTietPhieuKhams.Sum(c => c.SoLuong * c.DonGia);

        var hoaDon = new HoaDon
        {
            MaHoaDon = MaGenerator.Next("HD", await _db.HoaDons.Select(h => h.MaHoaDon).ToListAsync()),
            PhieuKhamId = phieu.Id,
            TienKham = tienKham,
            TienThuoc = tienThuoc,
            TongTien = tienKham + tienThuoc,
            NgayLap = VietnamTime.Today,
            DaThanhToan = true
        };
        _db.HoaDons.Add(hoaDon);
        await _db.SaveChangesAsync();

        return BuildDto(phieu, tienKham, hoaDon);
    }

    public async Task<HoaDonDto?> GetByMaAsync(string maHoaDon)
    {
        var hoaDon = await _db.HoaDons
            .AsNoTracking()
            .Include(h => h.PhieuKham)!.ThenInclude(p => p!.BenhNhan)
            .Include(h => h.PhieuKham)!.ThenInclude(p => p!.ChiTietPhieuKhams).ThenInclude(c => c.CachDung)
            .Include(h => h.PhieuKham)!.ThenInclude(p => p!.ChiTietPhieuKhams).ThenInclude(c => c.Thuoc)
            .FirstOrDefaultAsync(h => h.MaHoaDon == maHoaDon);

        if (hoaDon?.PhieuKham is null) return null;
        return BuildDto(hoaDon.PhieuKham, hoaDon.TienKham, hoaDon);
    }

    private static HoaDonDto BuildDto(PhieuKham phieu, decimal tienKham, HoaDon? hoaDon)
    {
        var chiTiet = phieu.ChiTietPhieuKhams.Select(c => new ChiTietToaThuocDto
        {
            MaThuoc = c.Thuoc?.MaThuoc ?? string.Empty,
            TenThuoc = c.TenThuoc,
            SoLuong = c.SoLuong,
            MaCachDung = c.CachDung?.MaCachDung ?? string.Empty,
            MoTaCachDung = c.CachDung?.MoTaCachDung ?? string.Empty,
            DonGia = c.DonGia,
            ThanhTien = c.SoLuong * c.DonGia
        }).ToList();

        var tienThuoc = chiTiet.Sum(x => x.ThanhTien);

        return new HoaDonDto
        {
            MaHoaDon = hoaDon?.MaHoaDon,
            MaPhieuKham = phieu.MaPhieuKham,
            MaBenhNhan = phieu.BenhNhan?.MaBenhNhan ?? string.Empty,
            HoTen = phieu.BenhNhan?.HoTen ?? string.Empty,
            NgayKham = phieu.NgayKham.ToDateTime(TimeOnly.MinValue),
            TienKham = hoaDon?.TienKham ?? tienKham,
            TienThuoc = hoaDon?.TienThuoc ?? tienThuoc,
            TongTien = hoaDon?.TongTien ?? (tienKham + tienThuoc),
            DaThanhToan = hoaDon?.DaThanhToan ?? false,
            ChiTietThuoc = chiTiet
        };
    }
}

using ClinicManagement.API.Common;
using ClinicManagement.API.Data;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Services;

/// <summary>YC2 – Lập phiếu khám bệnh (BM2, QĐ2).</summary>
public class PhieuKhamService : IPhieuKhamService
{
    private readonly ClinicDbContext _db;

    public PhieuKhamService(ClinicDbContext db) => _db = db;

    public async Task<PhieuKhamDto> CreateAsync(CreatePhieuKhamRequest request)
    {
        var benhNhan = await _db.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == request.MaBenhNhan)
            ?? throw new DomainException($"Không tìm thấy bệnh nhân '{request.MaBenhNhan}'.");

        var ngay = request.NgayKham == default
            ? VietnamTime.Today
            : DateOnly.FromDateTime(request.NgayKham);

        // Precondition: bệnh nhân phải có trong danh sách khám của ngày
        var chiTietDsk = await _db.ChiTietDanhSachKhams
            .Include(c => c.DanhSachKham)
            .FirstOrDefaultAsync(c => c.BenhNhanId == benhNhan.Id && c.DanhSachKham!.NgayKham == ngay);
        if (chiTietDsk is null)
            throw new DomainException("Bệnh nhân chưa có trong danh sách khám của ngày này.");

        // Loại bệnh (QĐ2: phải thuộc danh mục nếu có nhập)
        LoaiBenh? loaiBenh = null;
        if (!string.IsNullOrWhiteSpace(request.MaLoaiBenh))
        {
            loaiBenh = await _db.LoaiBenhs.FirstOrDefaultAsync(l => l.MaLoaiBenh == request.MaLoaiBenh)
                ?? throw new DomainException($"Loại bệnh '{request.MaLoaiBenh}' không có trong danh mục.");
        }

        var phieu = new PhieuKham
        {
            MaPhieuKham = MaGenerator.Next("PK", await _db.PhieuKhams.Select(p => p.MaPhieuKham).ToListAsync()),
            BenhNhanId = benhNhan.Id,
            NgayKham = ngay,
            TrieuChung = request.TrieuChung?.Trim(),
            LoaiBenhId = loaiBenh?.Id
        };

        // Toa thuốc (QĐ2: thuốc & cách dùng từ danh mục; số lượng nguyên dương)
        foreach (var item in request.ToaThuoc)
        {
            if (item.SoLuong <= 0)
                throw new DomainException("Số lượng thuốc phải là số nguyên dương (> 0).");

            var thuoc = await _db.Thuocs.FirstOrDefaultAsync(t => t.MaThuoc == item.MaThuoc)
                ?? throw new DomainException($"Thuốc '{item.MaThuoc}' không có trong danh mục.");
            var cachDung = await _db.CachDungs.FirstOrDefaultAsync(c => c.MaCachDung == item.MaCachDung)
                ?? throw new DomainException($"Cách dùng '{item.MaCachDung}' không có trong danh mục.");

            phieu.ChiTietPhieuKhams.Add(new ChiTietPhieuKham
            {
                ThuocId = thuoc.Id,
                TenThuoc = thuoc.TenThuoc,   // snapshot
                DonGia = thuoc.DonGia,       // snapshot
                SoLuong = item.SoLuong,
                CachDungId = cachDung.Id
            });
        }

        _db.PhieuKhams.Add(phieu);

        // Cập nhật trạng thái trong danh sách khám
        chiTietDsk.TrangThai = "Đã khám";

        await _db.SaveChangesAsync();

        return (await GetByMaAsync(phieu.MaPhieuKham))!;
    }

    public async Task<PhieuKhamDto?> GetByMaAsync(string maPhieuKham)
    {
        var phieu = await _db.PhieuKhams
            .AsNoTracking()
            .Include(p => p.BenhNhan)
            .Include(p => p.LoaiBenh)
            .Include(p => p.HoaDon)
            .Include(p => p.ChiTietPhieuKhams).ThenInclude(c => c.CachDung)
            .Include(p => p.ChiTietPhieuKhams).ThenInclude(c => c.Thuoc)
            .FirstOrDefaultAsync(p => p.MaPhieuKham == maPhieuKham);

        if (phieu is null) return null;

        return new PhieuKhamDto
        {
            MaPhieuKham = phieu.MaPhieuKham,
            MaBenhNhan = phieu.BenhNhan?.MaBenhNhan ?? string.Empty,
            HoTen = phieu.BenhNhan?.HoTen ?? string.Empty,
            NgayKham = phieu.NgayKham.ToDateTime(TimeOnly.MinValue),
            TrieuChung = phieu.TrieuChung,
            MaLoaiBenh = phieu.LoaiBenh?.MaLoaiBenh,
            TenLoaiBenh = phieu.LoaiBenh?.TenLoaiBenh,
            DaLapHoaDon = phieu.HoaDon != null,
            ToaThuoc = phieu.ChiTietPhieuKhams.Select(c => new ChiTietToaThuocDto
            {
                MaThuoc = c.Thuoc?.MaThuoc ?? string.Empty,
                TenThuoc = c.TenThuoc,
                SoLuong = c.SoLuong,
                MaCachDung = c.CachDung?.MaCachDung ?? string.Empty,
                MoTaCachDung = c.CachDung?.MoTaCachDung ?? string.Empty,
                DonGia = c.DonGia,
                ThanhTien = c.SoLuong * c.DonGia
            }).ToList()
        };
    }
}

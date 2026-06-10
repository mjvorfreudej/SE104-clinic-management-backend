using ClinicManagement.API.Data;
using ClinicManagement.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Services;

/// <summary>YC3 – Tra cứu bệnh nhân (BM3). Chỉ đọc, không thay đổi dữ liệu.</summary>
public class TraCuuService : ITraCuuService
{
    private readonly ClinicDbContext _db;

    public TraCuuService(ClinicDbContext db) => _db = db;

    public async Task<List<TraCuuBenhNhanResultDto>> TraCuuAsync(
        string? hoTen, int? namSinh, string? gioiTinh, DateTime? ngayKham)
    {
        // Truy vấn dựa trên phiếu khám (mỗi phiếu = 1 lần khám với loại bệnh/triệu chứng)
        var q = _db.PhieuKhams
            .AsNoTracking()
            .Include(p => p.BenhNhan)
            .Include(p => p.LoaiBenh)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(hoTen))
        {
            var key = hoTen.Trim().ToLower();
            q = q.Where(p => p.BenhNhan!.HoTen.ToLower().Contains(key));
        }
        if (namSinh.HasValue)
            q = q.Where(p => p.BenhNhan!.NamSinh == namSinh.Value);
        if (!string.IsNullOrWhiteSpace(gioiTinh))
            q = q.Where(p => p.BenhNhan!.GioiTinh.ToLower() == gioiTinh.Trim().ToLower());
        if (ngayKham.HasValue)
        {
            var ngay = DateOnly.FromDateTime(ngayKham.Value);
            q = q.Where(p => p.NgayKham == ngay);
        }

        var data = await q
            .OrderByDescending(p => p.NgayKham)
            .Select(p => new
            {
                p.BenhNhan!.MaBenhNhan,
                p.BenhNhan.HoTen,
                p.BenhNhan.GioiTinh,
                p.BenhNhan.NamSinh,
                p.NgayKham,
                TenLoaiBenh = p.LoaiBenh != null ? p.LoaiBenh.TenLoaiBenh : null,
                p.TrieuChung
            })
            .ToListAsync();

        var result = new List<TraCuuBenhNhanResultDto>();
        int stt = 1;
        foreach (var d in data)
        {
            result.Add(new TraCuuBenhNhanResultDto
            {
                STT = stt++,
                MaBenhNhan = d.MaBenhNhan,
                HoTen = d.HoTen,
                GioiTinh = d.GioiTinh,
                NamSinh = d.NamSinh,
                NgayKham = d.NgayKham.ToDateTime(TimeOnly.MinValue),
                TenLoaiBenh = d.TenLoaiBenh,
                TrieuChung = d.TrieuChung
            });
        }
        return result;
    }

    public async Task<List<LichSuKhamDto>> LichSuKhamAsync(string maBenhNhan)
    {
        return await _db.PhieuKhams
            .AsNoTracking()
            .Include(p => p.LoaiBenh)
            .Include(p => p.ChiTietPhieuKhams).ThenInclude(c => c.CachDung)
            .Include(p => p.ChiTietPhieuKhams).ThenInclude(c => c.Thuoc)
            .Where(p => p.BenhNhan!.MaBenhNhan == maBenhNhan)
            .OrderByDescending(p => p.NgayKham)
            .Select(p => new LichSuKhamDto
            {
                MaPhieuKham = p.MaPhieuKham,
                NgayKham = p.NgayKham.ToDateTime(TimeOnly.MinValue),
                TenLoaiBenh = p.LoaiBenh != null ? p.LoaiBenh.TenLoaiBenh : null,
                TrieuChung = p.TrieuChung,
                ToaThuoc = p.ChiTietPhieuKhams.Select(c => new ChiTietToaThuocDto
                {
                    MaThuoc = c.Thuoc!.MaThuoc,
                    TenThuoc = c.TenThuoc,
                    SoLuong = c.SoLuong,
                    MaCachDung = c.CachDung!.MaCachDung,
                    MoTaCachDung = c.CachDung.MoTaCachDung,
                    DonGia = c.DonGia,
                    ThanhTien = c.SoLuong * c.DonGia
                }).ToList()
            })
            .ToListAsync();
    }
}

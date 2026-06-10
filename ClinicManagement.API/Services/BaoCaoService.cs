using ClinicManagement.API.Common;
using ClinicManagement.API.Data;
using ClinicManagement.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Services;

/// <summary>YC5 – Lập báo cáo tháng (BM5.1 doanh thu, BM5.2 sử dụng thuốc).</summary>
public class BaoCaoService : IBaoCaoService
{
    private readonly ClinicDbContext _db;

    public BaoCaoService(ClinicDbContext db) => _db = db;

    private static (DateOnly First, DateOnly Last) ValidateAndRange(int thang, int nam)
    {
        if (thang is < 1 or > 12)
            throw new DomainException("Tháng phải nằm trong khoảng 1 đến 12.");

        // Không cho phép báo cáo tháng ở tương lai
        var now = VietnamTime.Now;
        if (nam > now.Year || (nam == now.Year && thang > now.Month))
            throw new DomainException("Không thể lập báo cáo cho tháng trong tương lai.");

        var first = new DateOnly(nam, thang, 1);
        var last = first.AddMonths(1).AddDays(-1);
        return (first, last);
    }

    public async Task<BaoCaoDoanhThuDto> DoanhThuAsync(int thang, int nam)
    {
        var (first, last) = ValidateAndRange(thang, nam);

        var hoaDons = await _db.HoaDons
            .AsNoTracking()
            .Where(h => h.NgayLap >= first && h.NgayLap <= last)
            .Select(h => new { h.NgayLap, h.TongTien })
            .ToListAsync();

        var tongDoanhThu = hoaDons.Sum(h => h.TongTien);

        var grouped = hoaDons
            .GroupBy(h => h.NgayLap)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Ngay = g.Key,
                SoBenhNhan = g.Count(),
                DoanhThu = g.Sum(x => x.TongTien)
            })
            .ToList();

        var dto = new BaoCaoDoanhThuDto
        {
            Thang = thang,
            Nam = nam,
            TongDoanhThu = tongDoanhThu,
            TongSoBenhNhan = grouped.Sum(g => g.SoBenhNhan)
        };

        int stt = 1;
        foreach (var g in grouped)
        {
            dto.ChiTiet.Add(new DoanhThuItemDto
            {
                STT = stt++,
                Ngay = g.Ngay.ToDateTime(TimeOnly.MinValue),
                SoBenhNhan = g.SoBenhNhan,
                DoanhThu = g.DoanhThu,
                TyLe = tongDoanhThu > 0 ? Math.Round((double)(g.DoanhThu / tongDoanhThu) * 100, 2) : 0
            });
        }
        return dto;
    }

    public async Task<BaoCaoSuDungThuocDto> SuDungThuocAsync(int thang, int nam)
    {
        var (first, last) = ValidateAndRange(thang, nam);

        var rows = await _db.ChiTietPhieuKhams
            .AsNoTracking()
            .Where(c => c.PhieuKham!.NgayKham >= first && c.PhieuKham.NgayKham <= last)
            .Select(c => new
            {
                c.ThuocId,
                c.TenThuoc,
                DonViTinh = c.Thuoc!.DonVi!.TenDonVi,
                c.SoLuong,
                c.PhieuKhamId
            })
            .ToListAsync();

        var grouped = rows
            .GroupBy(r => new { r.ThuocId, r.TenThuoc, r.DonViTinh })
            .Select(g => new
            {
                g.Key.TenThuoc,
                g.Key.DonViTinh,
                SoLuong = g.Sum(x => x.SoLuong),
                SoLanDung = g.Select(x => x.PhieuKhamId).Distinct().Count()
            })
            .OrderByDescending(x => x.SoLuong)
            .ToList();

        var dto = new BaoCaoSuDungThuocDto { Thang = thang, Nam = nam };
        int stt = 1;
        foreach (var g in grouped)
        {
            dto.ChiTiet.Add(new SuDungThuocItemDto
            {
                STT = stt++,
                TenThuoc = g.TenThuoc,
                DonViTinh = g.DonViTinh,
                SoLuong = g.SoLuong,
                SoLanDung = g.SoLanDung
            });
        }
        return dto;
    }
}

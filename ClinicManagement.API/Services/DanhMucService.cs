using ClinicManagement.API.Data;
using ClinicManagement.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Services;

/// <summary>Cung cấp danh mục dùng chung (loại bệnh, thuốc, đơn vị, cách dùng) cho YC2.</summary>
public class DanhMucService : IDanhMucService
{
    private readonly ClinicDbContext _db;

    public DanhMucService(ClinicDbContext db) => _db = db;

    public async Task<List<LoaiBenhDto>> GetLoaiBenhAsync() =>
        await _db.LoaiBenhs.AsNoTracking()
            .OrderBy(x => x.MaLoaiBenh)
            .Select(x => new LoaiBenhDto { Id = x.Id, MaLoaiBenh = x.MaLoaiBenh, TenLoaiBenh = x.TenLoaiBenh })
            .ToListAsync();

    public async Task<List<ThuocDto>> GetThuocAsync() =>
        await _db.Thuocs.AsNoTracking()
            .Include(x => x.DonVi)
            .OrderBy(x => x.MaThuoc)
            .Select(x => new ThuocDto
            {
                Id = x.Id,
                MaThuoc = x.MaThuoc,
                TenThuoc = x.TenThuoc,
                DonGia = x.DonGia,
                MaDonVi = x.DonVi!.MaDonVi,
                TenDonVi = x.DonVi.TenDonVi
            })
            .ToListAsync();

    public async Task<List<DonViDto>> GetDonViAsync() =>
        await _db.DonVis.AsNoTracking()
            .OrderBy(x => x.MaDonVi)
            .Select(x => new DonViDto { Id = x.Id, MaDonVi = x.MaDonVi, TenDonVi = x.TenDonVi })
            .ToListAsync();

    public async Task<List<CachDungDto>> GetCachDungAsync() =>
        await _db.CachDungs.AsNoTracking()
            .OrderBy(x => x.MaCachDung)
            .Select(x => new CachDungDto { Id = x.Id, MaCachDung = x.MaCachDung, MoTaCachDung = x.MoTaCachDung })
            .ToListAsync();
}

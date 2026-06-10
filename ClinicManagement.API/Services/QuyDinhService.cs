using ClinicManagement.API.Common;
using ClinicManagement.API.Data;
using ClinicManagement.API.DTOs;
using ClinicManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Services;

/// <summary>YC6 – Thay đổi quy định (QĐ1, QĐ2, QĐ4).</summary>
public class QuyDinhService : IQuyDinhService
{
    private readonly ClinicDbContext _db;

    public QuyDinhService(ClinicDbContext db) => _db = db;

    private async Task<ThamSo> GetOrCreateThamSoAsync()
    {
        var ts = await _db.ThamSos.FirstOrDefaultAsync();
        if (ts is null)
        {
            ts = new ThamSo { SoBenhNhanToiDaNgay = 40, TienKham = 30000m };
            _db.ThamSos.Add(ts);
            await _db.SaveChangesAsync();
        }
        return ts;
    }

    public async Task<ThamSoDto> GetThamSoAsync()
    {
        var ts = await GetOrCreateThamSoAsync();
        return new ThamSoDto
        {
            SoBenhNhanToiDaNgay = ts.SoBenhNhanToiDaNgay,
            TienKham = ts.TienKham,
            SoLoaiBenh = await _db.LoaiBenhs.CountAsync(),
            SoLoaiThuoc = await _db.Thuocs.CountAsync(),
            SoDonVi = await _db.DonVis.CountAsync(),
            SoCachDung = await _db.CachDungs.CountAsync()
        };
    }

    public async Task<ThamSoDto> UpdateThamSoAsync(UpdateThamSoRequest request)
    {
        if (request.SoBenhNhanToiDaNgay <= 0)
            throw new DomainException("Số bệnh nhân tối đa phải là số dương lớn hơn 0.");
        if (request.TienKham < 0)
            throw new DomainException("Tiền khám phải lớn hơn hoặc bằng 0.");

        var ts = await GetOrCreateThamSoAsync();
        ts.SoBenhNhanToiDaNgay = request.SoBenhNhanToiDaNgay;
        ts.TienKham = request.TienKham;
        await _db.SaveChangesAsync();

        return await GetThamSoAsync();
    }

    // ---------- Loại bệnh ----------

    public async Task<LoaiBenhDto> AddLoaiBenhAsync(UpsertLoaiBenhRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TenLoaiBenh))
            throw new DomainException("Tên loại bệnh không được để trống.");

        var ma = string.IsNullOrWhiteSpace(request.MaLoaiBenh)
            ? MaGenerator.Next("LB", await _db.LoaiBenhs.Select(x => x.MaLoaiBenh).ToListAsync())
            : request.MaLoaiBenh.Trim();

        if (await _db.LoaiBenhs.AnyAsync(x => x.MaLoaiBenh == ma))
            throw new DomainException($"Mã loại bệnh '{ma}' đã tồn tại.");

        var lb = new LoaiBenh { MaLoaiBenh = ma, TenLoaiBenh = request.TenLoaiBenh.Trim() };
        _db.LoaiBenhs.Add(lb);
        await _db.SaveChangesAsync();
        return new LoaiBenhDto { Id = lb.Id, MaLoaiBenh = lb.MaLoaiBenh, TenLoaiBenh = lb.TenLoaiBenh };
    }

    public async Task<LoaiBenhDto> UpdateLoaiBenhAsync(int id, UpsertLoaiBenhRequest request)
    {
        var lb = await _db.LoaiBenhs.FindAsync(id)
            ?? throw new DomainException("Không tìm thấy loại bệnh.");
        if (string.IsNullOrWhiteSpace(request.TenLoaiBenh))
            throw new DomainException("Tên loại bệnh không được để trống.");

        lb.TenLoaiBenh = request.TenLoaiBenh.Trim();
        await _db.SaveChangesAsync();
        return new LoaiBenhDto { Id = lb.Id, MaLoaiBenh = lb.MaLoaiBenh, TenLoaiBenh = lb.TenLoaiBenh };
    }

    public async Task DeleteLoaiBenhAsync(int id)
    {
        var lb = await _db.LoaiBenhs.FindAsync(id)
            ?? throw new DomainException("Không tìm thấy loại bệnh.");
        if (await _db.PhieuKhams.AnyAsync(p => p.LoaiBenhId == id))
            throw new DomainException("Không thể xóa: loại bệnh đang được dùng trong phiếu khám.");

        _db.LoaiBenhs.Remove(lb);
        await _db.SaveChangesAsync();
    }

    // ---------- Thuốc ----------

    public async Task<ThuocDto> AddThuocAsync(UpsertThuocRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TenThuoc))
            throw new DomainException("Tên thuốc không được để trống.");
        if (request.DonGia < 0)
            throw new DomainException("Đơn giá phải lớn hơn hoặc bằng 0.");

        var donVi = await _db.DonVis.FirstOrDefaultAsync(d => d.MaDonVi == request.MaDonVi)
            ?? throw new DomainException($"Không tìm thấy đơn vị '{request.MaDonVi}'.");

        var ma = string.IsNullOrWhiteSpace(request.MaThuoc)
            ? MaGenerator.Next("T", await _db.Thuocs.Select(x => x.MaThuoc).ToListAsync())
            : request.MaThuoc.Trim();

        if (await _db.Thuocs.AnyAsync(x => x.MaThuoc == ma))
            throw new DomainException($"Mã thuốc '{ma}' đã tồn tại.");

        var thuoc = new Thuoc
        {
            MaThuoc = ma,
            TenThuoc = request.TenThuoc.Trim(),
            DonGia = request.DonGia,
            DonViId = donVi.Id
        };
        _db.Thuocs.Add(thuoc);
        await _db.SaveChangesAsync();

        return ToThuocDto(thuoc, donVi);
    }

    public async Task<ThuocDto> UpdateThuocAsync(int id, UpsertThuocRequest request)
    {
        var thuoc = await _db.Thuocs.Include(t => t.DonVi).FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new DomainException("Không tìm thấy thuốc.");
        if (string.IsNullOrWhiteSpace(request.TenThuoc))
            throw new DomainException("Tên thuốc không được để trống.");
        if (request.DonGia < 0)
            throw new DomainException("Đơn giá phải lớn hơn hoặc bằng 0.");

        var donVi = await _db.DonVis.FirstOrDefaultAsync(d => d.MaDonVi == request.MaDonVi)
            ?? throw new DomainException($"Không tìm thấy đơn vị '{request.MaDonVi}'.");

        thuoc.TenThuoc = request.TenThuoc.Trim();
        thuoc.DonGia = request.DonGia; // chỉ áp dụng cho phiếu khám MỚI (lịch sử đã snapshot DonGia)
        thuoc.DonViId = donVi.Id;
        await _db.SaveChangesAsync();

        return ToThuocDto(thuoc, donVi);
    }

    public async Task DeleteThuocAsync(int id)
    {
        var thuoc = await _db.Thuocs.FindAsync(id)
            ?? throw new DomainException("Không tìm thấy thuốc.");
        if (await _db.ChiTietPhieuKhams.AnyAsync(c => c.ThuocId == id))
            throw new DomainException("Không thể xóa: thuốc đã được kê trong phiếu khám.");

        _db.Thuocs.Remove(thuoc);
        await _db.SaveChangesAsync();
    }

    private static ThuocDto ToThuocDto(Thuoc t, DonVi dv) => new()
    {
        Id = t.Id,
        MaThuoc = t.MaThuoc,
        TenThuoc = t.TenThuoc,
        DonGia = t.DonGia,
        MaDonVi = dv.MaDonVi,
        TenDonVi = dv.TenDonVi
    };

    // ---------- Đơn vị tính ----------

    public async Task<DonViDto> AddDonViAsync(UpsertDonViRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TenDonVi))
            throw new DomainException("Tên đơn vị tính không được để trống.");

        var ma = string.IsNullOrWhiteSpace(request.MaDonVi)
            ? MaGenerator.Next("DV", await _db.DonVis.Select(x => x.MaDonVi).ToListAsync())
            : request.MaDonVi.Trim();

        if (await _db.DonVis.AnyAsync(x => x.MaDonVi == ma))
            throw new DomainException($"Mã đơn vị '{ma}' đã tồn tại.");

        var dv = new DonVi { MaDonVi = ma, TenDonVi = request.TenDonVi.Trim() };
        _db.DonVis.Add(dv);
        await _db.SaveChangesAsync();
        return new DonViDto { Id = dv.Id, MaDonVi = dv.MaDonVi, TenDonVi = dv.TenDonVi };
    }

    public async Task<DonViDto> UpdateDonViAsync(int id, UpsertDonViRequest request)
    {
        var dv = await _db.DonVis.FindAsync(id)
            ?? throw new DomainException("Không tìm thấy đơn vị tính.");
        if (string.IsNullOrWhiteSpace(request.TenDonVi))
            throw new DomainException("Tên đơn vị tính không được để trống.");

        dv.TenDonVi = request.TenDonVi.Trim();
        await _db.SaveChangesAsync();
        return new DonViDto { Id = dv.Id, MaDonVi = dv.MaDonVi, TenDonVi = dv.TenDonVi };
    }

    public async Task DeleteDonViAsync(int id)
    {
        var dv = await _db.DonVis.FindAsync(id)
            ?? throw new DomainException("Không tìm thấy đơn vị tính.");
        if (await _db.Thuocs.AnyAsync(t => t.DonViId == id))
            throw new DomainException("Không thể xóa: đơn vị tính đang được dùng bởi thuốc.");

        _db.DonVis.Remove(dv);
        await _db.SaveChangesAsync();
    }

    // ---------- Cách dùng ----------

    public async Task<CachDungDto> AddCachDungAsync(UpsertCachDungRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MoTaCachDung))
            throw new DomainException("Mô tả cách dùng không được để trống.");

        var ma = string.IsNullOrWhiteSpace(request.MaCachDung)
            ? MaGenerator.Next("CD", await _db.CachDungs.Select(x => x.MaCachDung).ToListAsync())
            : request.MaCachDung.Trim();

        if (await _db.CachDungs.AnyAsync(x => x.MaCachDung == ma))
            throw new DomainException($"Mã cách dùng '{ma}' đã tồn tại.");

        var cd = new CachDung { MaCachDung = ma, MoTaCachDung = request.MoTaCachDung.Trim() };
        _db.CachDungs.Add(cd);
        await _db.SaveChangesAsync();
        return new CachDungDto { Id = cd.Id, MaCachDung = cd.MaCachDung, MoTaCachDung = cd.MoTaCachDung };
    }

    public async Task<CachDungDto> UpdateCachDungAsync(int id, UpsertCachDungRequest request)
    {
        var cd = await _db.CachDungs.FindAsync(id)
            ?? throw new DomainException("Không tìm thấy cách dùng.");
        if (string.IsNullOrWhiteSpace(request.MoTaCachDung))
            throw new DomainException("Mô tả cách dùng không được để trống.");

        cd.MoTaCachDung = request.MoTaCachDung.Trim();
        await _db.SaveChangesAsync();
        return new CachDungDto { Id = cd.Id, MaCachDung = cd.MaCachDung, MoTaCachDung = cd.MoTaCachDung };
    }

    public async Task DeleteCachDungAsync(int id)
    {
        var cd = await _db.CachDungs.FindAsync(id)
            ?? throw new DomainException("Không tìm thấy cách dùng.");
        if (await _db.ChiTietPhieuKhams.AnyAsync(c => c.CachDungId == id))
            throw new DomainException("Không thể xóa: cách dùng đang được dùng trong phiếu khám.");

        _db.CachDungs.Remove(cd);
        await _db.SaveChangesAsync();
    }
}

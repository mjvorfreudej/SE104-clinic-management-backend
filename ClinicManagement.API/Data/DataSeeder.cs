using ClinicManagement.API.Common;
using ClinicManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Data;

/// <summary>
/// Khởi tạo dữ liệu mặc định (idempotent – chạy nhiều lần không nhân đôi dữ liệu):
/// vai trò, tài khoản mẫu, danh mục (QĐ2) và tham số hệ thống (QĐ1/QĐ4).
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(ClinicDbContext db)
    {
        await SeedVaiTroAsync(db);
        await SeedNguoiDungAsync(db);
        await SeedDanhMucAsync(db);
        await SeedThamSoAsync(db);
        await db.SaveChangesAsync();
    }

    private static async Task SeedVaiTroAsync(ClinicDbContext db)
    {
        if (await db.VaiTros.AnyAsync()) return;

        db.VaiTros.AddRange(
            new VaiTro { Code = VaiTroCode.Admin, TenVaiTro = "Admin" },
            new VaiTro { Code = VaiTroCode.BacSi, TenVaiTro = "Bác Sĩ" },
            new VaiTro { Code = VaiTroCode.TiepTan, TenVaiTro = "Tiếp Tân" },
            new VaiTro { Code = VaiTroCode.KeToan, TenVaiTro = "Kế Toán" }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedNguoiDungAsync(ClinicDbContext db)
    {
        if (await db.NguoiDungs.AnyAsync()) return;

        var roles = await db.VaiTros.ToDictionaryAsync(v => v.Code, v => v.Id);

        // Tài khoản demo – mật khẩu mặc định khai báo ngay bên dưới
        var users = new[]
        {
            ("admin@clinic.com",   "Admin@123",   "Quản Trị Viên", VaiTroCode.Admin),
            ("bacsi@clinic.com",   "Bacsi@123",   "BS. Nguyễn An", VaiTroCode.BacSi),
            ("tieptan@clinic.com", "Tieptan@123", "Lễ Tân Mai",    VaiTroCode.TiepTan),
            ("ketoan@clinic.com",  "Ketoan@123",  "Kế Toán Lan",   VaiTroCode.KeToan),
        };

        foreach (var (email, matKhau, hoTen, roleCode) in users)
        {
            db.NguoiDungs.Add(new NguoiDung
            {
                TenDangNhap = email,
                HoTen = hoTen,
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword(matKhau),
                VaiTroId = roles[roleCode],
                KichHoat = true
            });
        }
        await db.SaveChangesAsync();
    }

    private static async Task SeedDanhMucAsync(ClinicDbContext db)
    {
        // ----- Đơn vị tính (QĐ2: 2 đơn vị) -----
        if (!await db.DonVis.AnyAsync())
        {
            db.DonVis.AddRange(
                new DonVi { MaDonVi = "DV01", TenDonVi = "Viên" },
                new DonVi { MaDonVi = "DV02", TenDonVi = "Chai" }
            );
            await db.SaveChangesAsync();
        }

        // ----- Cách dùng (QĐ2: 4 cách dùng) -----
        if (!await db.CachDungs.AnyAsync())
        {
            db.CachDungs.AddRange(
                new CachDung { MaCachDung = "CD1", MoTaCachDung = "Ngày 1 lần sau ăn" },
                new CachDung { MaCachDung = "CD2", MoTaCachDung = "Ngày 2 lần sáng - tối" },
                new CachDung { MaCachDung = "CD3", MoTaCachDung = "Ngày 3 lần sau ăn" },
                new CachDung { MaCachDung = "CD4", MoTaCachDung = "Ngày 4 lần theo chỉ định" }
            );
            await db.SaveChangesAsync();
        }

        // ----- Loại bệnh (QĐ2: 5 loại bệnh) -----
        if (!await db.LoaiBenhs.AnyAsync())
        {
            db.LoaiBenhs.AddRange(
                new LoaiBenh { MaLoaiBenh = "LB1", TenLoaiBenh = "Cảm cúm" },
                new LoaiBenh { MaLoaiBenh = "LB2", TenLoaiBenh = "Viêm họng" },
                new LoaiBenh { MaLoaiBenh = "LB3", TenLoaiBenh = "Sốt xuất huyết" },
                new LoaiBenh { MaLoaiBenh = "LB4", TenLoaiBenh = "Đau dạ dày" },
                new LoaiBenh { MaLoaiBenh = "LB5", TenLoaiBenh = "Tiêu chảy cấp" }
            );
            await db.SaveChangesAsync();
        }

        // ----- Thuốc (QĐ2: 30 loại thuốc, mỗi loại có đơn giá riêng - QĐ4) -----
        if (!await db.Thuocs.AnyAsync())
        {
            var vien = await db.DonVis.FirstAsync(d => d.MaDonVi == "DV01");
            var chai = await db.DonVis.FirstAsync(d => d.MaDonVi == "DV02");

            // (Tên, Đơn giá, Đơn vị)  – viên hoặc chai
            var thuocData = new (string Ten, decimal Gia, bool LaVien)[]
            {
                ("Paracetamol 500mg", 1500, true),
                ("Amoxicillin 500mg", 2500, true),
                ("Vitamin C 1000mg", 1200, true),
                ("Ibuprofen 400mg", 2000, true),
                ("Cefuroxime 250mg", 5000, true),
                ("Loratadine 10mg", 1800, true),
                ("Omeprazole 20mg", 3000, true),
                ("Aspirin 81mg", 1000, true),
                ("Cetirizine 10mg", 1700, true),
                ("Metformin 500mg", 2200, true),
                ("Berberin", 800, true),
                ("Smecta", 4000, true),
                ("Oresol", 6000, true),
                ("Azithromycin 500mg", 8000, true),
                ("Dexamethasone 0.5mg", 1300, true),
                ("Prednisolone 5mg", 1400, true),
                ("Salbutamol 4mg", 1600, true),
                ("Domperidone 10mg", 1900, true),
                ("Diclofenac 50mg", 2100, true),
                ("Clarithromycin 500mg", 9000, true),
                ("Siro ho Bổ phế", 35000, false),
                ("Siro Prospan", 65000, false),
                ("Dung dịch sát khuẩn Betadine", 28000, false),
                ("Nước muối sinh lý 0.9%", 12000, false),
                ("Siro ho Astex", 32000, false),
                ("Dầu gió xanh", 18000, false),
                ("Siro Ambroxol", 30000, false),
                ("Dung dịch nhỏ mắt NaCl", 15000, false),
                ("Siro Ferrovit (bổ máu)", 45000, false),
                ("Men tiêu hóa Enterogermina", 55000, false),
            };

            int i = 1;
            foreach (var (ten, gia, laVien) in thuocData)
            {
                db.Thuocs.Add(new Thuoc
                {
                    MaThuoc = $"T{i:D3}",
                    TenThuoc = ten,
                    DonGia = gia,
                    DonViId = laVien ? vien.Id : chai.Id
                });
                i++;
            }
            await db.SaveChangesAsync();
        }
    }

    private static async Task SeedThamSoAsync(ClinicDbContext db)
    {
        if (await db.ThamSos.AnyAsync()) return;

        // QĐ1: tối đa 40 BN/ngày; QĐ4: tiền khám 30.000đ
        db.ThamSos.Add(new ThamSo { SoBenhNhanToiDaNgay = 40, TienKham = 30000m });
        await db.SaveChangesAsync();
    }
}

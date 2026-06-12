using ClinicManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Data;

public class ClinicDbContext : DbContext
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

    public DbSet<VaiTro> VaiTros => Set<VaiTro>();
    public DbSet<NguoiDung> NguoiDungs => Set<NguoiDung>();
    public DbSet<BenhNhan> BenhNhans => Set<BenhNhan>();
    public DbSet<DanhSachKham> DanhSachKhams => Set<DanhSachKham>();
    public DbSet<ChiTietDanhSachKham> ChiTietDanhSachKhams => Set<ChiTietDanhSachKham>();
    public DbSet<LoaiBenh> LoaiBenhs => Set<LoaiBenh>();
    public DbSet<DonVi> DonVis => Set<DonVi>();
    public DbSet<CachDung> CachDungs => Set<CachDung>();
    public DbSet<Thuoc> Thuocs => Set<Thuoc>();
    public DbSet<PhieuKham> PhieuKhams => Set<PhieuKham>();
    public DbSet<ChiTietPhieuKham> ChiTietPhieuKhams => Set<ChiTietPhieuKham>();
    public DbSet<HoaDon> HoaDons => Set<HoaDon>();
    public DbSet<ThamSo> ThamSos => Set<ThamSo>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // ===== VaiTro =====
        b.Entity<VaiTro>(e =>
        {
            e.ToTable("VaiTro");
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.Code).HasMaxLength(30).IsRequired();
            e.Property(x => x.TenVaiTro).HasMaxLength(100).IsRequired();
        });

        // ===== NguoiDung =====
        b.Entity<NguoiDung>(e =>
        {
            e.ToTable("NguoiDung");
            e.HasIndex(x => x.TenDangNhap).IsUnique();
            e.Property(x => x.TenDangNhap).HasMaxLength(150).IsRequired();
            e.Property(x => x.MatKhauHash).IsRequired();
            e.Property(x => x.HoTen).HasMaxLength(150).IsRequired();
            e.HasOne(x => x.VaiTro)
             .WithMany(v => v.NguoiDungs)
             .HasForeignKey(x => x.VaiTroId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== BenhNhan =====
        b.Entity<BenhNhan>(e =>
        {
            e.ToTable("BenhNhan");
            e.HasIndex(x => x.MaBenhNhan).IsUnique();
            e.Property(x => x.MaBenhNhan).HasMaxLength(20).IsRequired();
            e.Property(x => x.HoTen).HasMaxLength(150).IsRequired();
            e.Property(x => x.GioiTinh).HasMaxLength(10);
            e.Property(x => x.DiaChi).HasMaxLength(300);
            e.Property(x => x.SoDienThoai).HasMaxLength(20);
            // Index (không unique – SĐT có thể trống/trùng) để tăng tốc tra cứu hồ sơ cũ.
            e.HasIndex(x => x.SoDienThoai);
        });

        // ===== DanhSachKham =====
        b.Entity<DanhSachKham>(e =>
        {
            e.ToTable("DanhSachKham");
            e.HasIndex(x => x.NgayKham).IsUnique();
        });

        // ===== ChiTietDanhSachKham =====
        b.Entity<ChiTietDanhSachKham>(e =>
        {
            e.ToTable("ChiTietDanhSachKham");
            e.Property(x => x.TrangThai).HasMaxLength(30);
            e.HasOne(x => x.DanhSachKham)
             .WithMany(d => d.ChiTietDanhSachKhams)
             .HasForeignKey(x => x.DanhSachKhamId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.BenhNhan)
             .WithMany(p => p.ChiTietDanhSachKhams)
             .HasForeignKey(x => x.BenhNhanId)
             .OnDelete(DeleteBehavior.Restrict);
            // Một bệnh nhân chỉ có 1 dòng trong danh sách của 1 ngày
            e.HasIndex(x => new { x.DanhSachKhamId, x.BenhNhanId }).IsUnique();
        });

        // ===== LoaiBenh =====
        b.Entity<LoaiBenh>(e =>
        {
            e.ToTable("LoaiBenh");
            e.HasIndex(x => x.MaLoaiBenh).IsUnique();
            e.Property(x => x.MaLoaiBenh).HasMaxLength(20).IsRequired();
            e.Property(x => x.TenLoaiBenh).HasMaxLength(150).IsRequired();
        });

        // ===== DonVi =====
        b.Entity<DonVi>(e =>
        {
            e.ToTable("DonVi");
            e.HasIndex(x => x.MaDonVi).IsUnique();
            e.Property(x => x.MaDonVi).HasMaxLength(20).IsRequired();
            e.Property(x => x.TenDonVi).HasMaxLength(50).IsRequired();
        });

        // ===== CachDung =====
        b.Entity<CachDung>(e =>
        {
            e.ToTable("CachDung");
            e.HasIndex(x => x.MaCachDung).IsUnique();
            e.Property(x => x.MaCachDung).HasMaxLength(20).IsRequired();
            e.Property(x => x.MoTaCachDung).HasMaxLength(200).IsRequired();
        });

        // ===== Thuoc =====
        b.Entity<Thuoc>(e =>
        {
            e.ToTable("Thuoc");
            e.HasIndex(x => x.MaThuoc).IsUnique();
            e.Property(x => x.MaThuoc).HasMaxLength(20).IsRequired();
            e.Property(x => x.TenThuoc).HasMaxLength(150).IsRequired();
            e.Property(x => x.DonGia).HasColumnType("numeric(18,2)");
            e.HasOne(x => x.DonVi)
             .WithMany(d => d.Thuocs)
             .HasForeignKey(x => x.DonViId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== PhieuKham =====
        b.Entity<PhieuKham>(e =>
        {
            e.ToTable("PhieuKham");
            e.HasIndex(x => x.MaPhieuKham).IsUnique();
            e.Property(x => x.MaPhieuKham).HasMaxLength(20).IsRequired();
            e.Property(x => x.TrieuChung).HasMaxLength(500);
            e.HasOne(x => x.BenhNhan)
             .WithMany(p => p.PhieuKhams)
             .HasForeignKey(x => x.BenhNhanId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.LoaiBenh)
             .WithMany(l => l.PhieuKhams)
             .HasForeignKey(x => x.LoaiBenhId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ===== ChiTietPhieuKham =====
        b.Entity<ChiTietPhieuKham>(e =>
        {
            e.ToTable("ChiTietPhieuKham");
            e.Ignore(x => x.ThanhTien);
            e.Property(x => x.TenThuoc).HasMaxLength(150).IsRequired();
            e.Property(x => x.DonGia).HasColumnType("numeric(18,2)");
            e.HasOne(x => x.PhieuKham)
             .WithMany(p => p.ChiTietPhieuKhams)
             .HasForeignKey(x => x.PhieuKhamId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Thuoc)
             .WithMany(t => t.ChiTietPhieuKhams)
             .HasForeignKey(x => x.ThuocId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.CachDung)
             .WithMany(c => c.ChiTietPhieuKhams)
             .HasForeignKey(x => x.CachDungId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== HoaDon =====
        b.Entity<HoaDon>(e =>
        {
            e.ToTable("HoaDon");
            e.HasIndex(x => x.MaHoaDon).IsUnique();
            e.Property(x => x.MaHoaDon).HasMaxLength(20).IsRequired();
            e.Property(x => x.TienKham).HasColumnType("numeric(18,2)");
            e.Property(x => x.TienThuoc).HasColumnType("numeric(18,2)");
            e.Property(x => x.TongTien).HasColumnType("numeric(18,2)");
            // 1 phiếu khám – 1 hóa đơn (tránh thu tiền 2 lần – QĐ4)
            // Quan hệ 1-1 đã tự tạo unique index trên PhieuKhamId (tránh thu tiền 2 lần – QĐ4)
            e.HasOne(x => x.PhieuKham)
             .WithOne(p => p.HoaDon)
             .HasForeignKey<HoaDon>(x => x.PhieuKhamId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== ThamSo =====
        b.Entity<ThamSo>(e =>
        {
            e.ToTable("ThamSo");
            e.Property(x => x.TienKham).HasColumnType("numeric(18,2)");
        });
    }
}

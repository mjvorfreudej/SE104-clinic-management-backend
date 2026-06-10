# Clinic Management – Backend API

Backend cho đồ án **Quản lý phòng mạch tư** (môn SE104 – Nhập môn CNPM).
Frontend (WPF) nằm ở repo riêng: [SE104---ClinicManagement](https://github.com/quackii00/SE104---ClinicManagement).

## 1. Công nghệ

| Thành phần | Công nghệ |
| ---------- | --------- |
| Framework | ASP.NET Core Web API (.NET 10) |
| ORM | Entity Framework Core + Npgsql |
| Database | Supabase PostgreSQL |
| Auth | JWT Bearer + phân quyền theo vai trò |
| Mật khẩu | BCrypt hash |
| Tài liệu API | Swagger / OpenAPI |
| Deploy | Docker → Render |

## 2. Cấu trúc

```
SE104-clinic-management-backend/
├── ClinicManagement.API/
│   ├── Common/          (DomainException, VaiTroCode, MaGenerator, VietnamTime)
│   ├── Configurations/  (JwtSettings)
│   ├── Controllers/     (Auth, DanhSachKham, PhieuKham, TraCuu, HoaDon, BaoCao, QuyDinh, DanhMuc)
│   ├── Data/            (ClinicDbContext, DataSeeder)
│   ├── DTOs/
│   ├── Middleware/      (ExceptionHandlingMiddleware)
│   ├── Models/          (13 entity)
│   ├── Services/        (logic nghiệp vụ YC1–YC6 + Auth/Token)
│   └── Program.cs
├── ClinicManagement.API.slnx
├── Dockerfile
└── render.yaml
```

> Kiến trúc: **Controller → Service → DbContext (EF Core)**. `DbSet` đóng vai trò repository; logic nghiệp vụ và validate nằm ở tầng Service.

## 3. Chạy ở máy local

```powershell
# 1. Điền connection string Supabase vào ClinicManagement.API/appsettings.Development.json
#    (key ConnectionStrings:DefaultConnection)
# 2. Chạy:
cd ClinicManagement.API
dotnet run
# 3. Mở http://localhost:5129/swagger
```

Lần đầu chạy, app **tự tạo bảng** (`EnsureCreated`) và **seed** dữ liệu mẫu (vai trò, tài khoản demo, danh mục, tham số). Không cần chạy SQL thủ công.

## 4. Deploy

Đã có sẵn `Dockerfile` + `render.yaml` để deploy lên Render bằng Docker.
Khi tạo service trên Render, cấu hình các biến môi trường ở **mục 7** (đặc biệt `ConnectionStrings__DefaultConnection` của Supabase và `Jwt__Key`).

## 5. Tài khoản demo (seed sẵn)

| Email | Mật khẩu | Vai trò |
|-------|----------|---------|
| `admin@clinic.com` | `Admin@123` | Admin |
| `bacsi@clinic.com` | `Bacsi@123` | Bác Sĩ |
| `tieptan@clinic.com` | `Tieptan@123` | Tiếp Tân |
| `ketoan@clinic.com` | `Ketoan@123` | Kế Toán |

## 6. API & phân quyền (tóm tắt)

| Module | Endpoint | Vai trò |
|--------|----------|---------|
| Auth | `POST /api/auth/login`, `GET /api/auth/me` | công khai / đã đăng nhập |
| Danh mục | `GET /api/danhmuc/{loaibenh\|thuoc\|donvi\|cachdung}` | đã đăng nhập |
| YC1 | `GET /api/danhsachkham/today`, `POST /api/danhsachkham/tiepnhan` | Tiếp Tân, Admin |
| YC2 | `POST /api/phieukham`, `GET /api/phieukham/{ma}` | Bác Sĩ, Admin |
| YC3 | `GET /api/tracuu/benhnhan`, `.../lichsu` | đã đăng nhập |
| YC4 | `POST /api/hoadon`, `GET /api/hoadon/preview` | Tiếp Tân, Admin |
| YC5 | `GET /api/baocao/{doanhthu\|sudungthuoc}` | Kế Toán, Admin |
| YC6 | `GET/PUT /api/quydinh`, CRUD `/api/quydinh/{loaibenh\|thuoc\|donvi\|cachdung}` | Admin |

## 7. Cấu hình (biến môi trường)

| Key | Mô tả |
|-----|-------|
| `ConnectionStrings__DefaultConnection` | Chuỗi kết nối Supabase PostgreSQL |
| `Jwt__Key` | Khóa ký JWT (≥ 32 ký tự) |
| `Jwt__Issuer` / `Jwt__Audience` | `ClinicManagement.API` / `ClinicManagement.UI` |
| `Jwt__ExpireMinutes` | Thời hạn token (mặc định 480) |

using System.Security.Claims;
using System.Text;
using ClinicManagement.API.Configurations;
using ClinicManagement.API.Data;
using ClinicManagement.API.Middleware;
using ClinicManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---- Lắng nghe đúng cổng Render cấp qua biến môi trường PORT ----
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// ===================== Services =====================

builder.Services.AddControllers();

// ---- Database (Supabase PostgreSQL qua Npgsql) ----
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

builder.Services.AddDbContext<ClinicDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseNpgsql(connectionString, npg => npg.EnableRetryOnFailure(3));
    }
});

// ---- JWT ----
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    // Khóa mặc định CHỈ dùng khi chạy local nếu chưa cấu hình. Production BẮT BUỘC set biến môi trường Jwt__Key.
    jwtSettings.Key = "DEV_ONLY_super_secret_key_change_me_please_min_32_chars!!";
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };
    });
builder.Services.AddAuthorization();

// ---- CORS ----
// Client là ứng dụng WPF Desktop (HttpClient không gửi Origin) nên không cần CORS để chạy.
// Chỉ mở AnyOrigin khi DEV hoặc khi cấu hình rõ Cors:AllowAnyOrigin=true; ngược lại giới hạn
// theo danh sách Cors:AllowedOrigins để tránh để API mở hoàn toàn ngoài Internet.
const string CorsPolicy = "AllowClient";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var corsAllowAny = builder.Configuration.GetValue<bool>("Cors:AllowAnyOrigin")
                   || builder.Environment.IsDevelopment();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        if (corsAllowAny)
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        else if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        // Không origin nào được cấu hình ở Production => không cho phép cross-origin (Desktop vẫn chạy bình thường).
    });
});

// ---- DI các service nghiệp vụ ----
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDanhSachKhamService, DanhSachKhamService>();
builder.Services.AddScoped<IPhieuKhamService, PhieuKhamService>();
builder.Services.AddScoped<ITraCuuService, TraCuuService>();
builder.Services.AddScoped<IHoaDonService, HoaDonService>();
builder.Services.AddScoped<IBaoCaoService, BaoCaoService>();
builder.Services.AddScoped<IQuyDinhService, QuyDinhService>();
builder.Services.AddScoped<IDanhMucService, DanhMucService>();

// ---- Swagger (bật ở mọi môi trường để dễ test trên cloud) ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinic Management API", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT (chỉ token, không cần chữ 'Bearer ')",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
});

var app = builder.Build();

// ===================== Pipeline =====================

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger: bật ở DEV, hoặc khi đặt Swagger:Enabled=true (vd để test trên cloud). Mặc định TẮT ở Production.
var enableSwagger = app.Environment.IsDevelopment()
                    || app.Configuration.GetValue<bool>("Swagger:Enabled");
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic Management API v1"));
}

app.UseCors(CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Endpoint kiểm tra sống & điều hướng trang chủ (về Swagger nếu bật, ngược lại về /health).
app.MapGet("/", () => Results.Redirect(enableSwagger ? "/swagger" : "/health"));
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));

// ===================== Khởi tạo CSDL + seed dữ liệu =====================
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        logger.LogWarning("Chưa cấu hình ConnectionStrings:DefaultConnection. Bỏ qua khởi tạo CSDL.");
    }
    else
    {
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();

            // Supabase có bảng nội bộ trong public schema làm EnsureCreated bỏ qua tạo bảng của ta.
            // Kiểm tra bảng VaiTro bằng SQL thuần; nếu chưa có thì tạo toàn bộ schema.
            var dbConn = db.Database.GetDbConnection();
            if (dbConn.State != System.Data.ConnectionState.Open)
                await dbConn.OpenAsync();
            await using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='VaiTro')";
                var exists = (bool)(await cmd.ExecuteScalarAsync() ?? false);
                if (!exists)
                {
                    // Tạo schema bằng SQL script EF sinh ra (chạy thẳng, không qua EnsureCreated)
                    var script = db.Database.GenerateCreateScript();
                    await db.Database.ExecuteSqlRawAsync(script);
                }
            }

            // Tiến hóa schema nhẹ (idempotent) cho CSDL đã tồn tại: thêm cột SĐT bệnh nhân nếu chưa có.
            // (Bản bootstrap GenerateCreateScript chỉ chạy khi DB trống, nên CSDL cũ cần ALTER bổ sung.)
            await db.Database.ExecuteSqlRawAsync(
                "ALTER TABLE \"BenhNhan\" ADD COLUMN IF NOT EXISTS \"SoDienThoai\" character varying(20); " +
                "CREATE INDEX IF NOT EXISTS \"IX_BenhNhan_SoDienThoai\" ON \"BenhNhan\" (\"SoDienThoai\");");

            // Tối ưu hóa mô hình dữ liệu về mặt thời gian (Thiết kế dữ liệu mục 3): snapshot trực tiếp
            // đơn vị tính & cách dùng vào ChiTietPhieuKham để giảm JOIN khi truy vấn/báo cáo.
            // Thêm cột (NOT NULL DEFAULT '' để an toàn với dữ liệu cũ) rồi backfill từ danh mục hiện hành.
            await db.Database.ExecuteSqlRawAsync(
                "ALTER TABLE \"ChiTietPhieuKham\" ADD COLUMN IF NOT EXISTS \"TenDonVi\" character varying(50) NOT NULL DEFAULT ''; " +
                "ALTER TABLE \"ChiTietPhieuKham\" ADD COLUMN IF NOT EXISTS \"TenCachDung\" character varying(200) NOT NULL DEFAULT ''; " +
                "UPDATE \"ChiTietPhieuKham\" c SET \"TenDonVi\" = dv.\"TenDonVi\" " +
                "FROM \"Thuoc\" t JOIN \"DonVi\" dv ON dv.\"Id\" = t.\"DonViId\" " +
                "WHERE c.\"ThuocId\" = t.\"Id\" AND (c.\"TenDonVi\" IS NULL OR c.\"TenDonVi\" = ''); " +
                "UPDATE \"ChiTietPhieuKham\" c SET \"TenCachDung\" = cd.\"MoTaCachDung\" " +
                "FROM \"CachDung\" cd " +
                "WHERE c.\"CachDungId\" = cd.\"Id\" AND (c.\"TenCachDung\" IS NULL OR c.\"TenCachDung\" = '');");

            await DataSeeder.SeedAsync(db);
            logger.LogInformation("Khởi tạo CSDL & seed dữ liệu thành công.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khởi tạo CSDL. Kiểm tra lại connection string Supabase.");
        }
    }
}

app.Run();

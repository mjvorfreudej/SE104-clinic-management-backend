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

// ---- CORS (cho phép Frontend / công cụ test gọi qua Internet) ----
const string CorsPolicy = "AllowClient";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
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

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic Management API v1"));

app.UseCors(CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Endpoint kiểm tra sống & điều hướng trang chủ về Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));
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
            await db.Database.EnsureCreatedAsync();
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

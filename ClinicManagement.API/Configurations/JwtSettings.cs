namespace ClinicManagement.API.Configurations;

/// <summary>Cấu hình JWT, nạp từ section "Jwt" trong appsettings / biến môi trường.</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ClinicManagement.API";
    public string Audience { get; set; } = "ClinicManagement.UI";
    public int ExpireMinutes { get; set; } = 480; // 8 giờ
}

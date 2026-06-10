using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicManagement.API.Configurations;
using ClinicManagement.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ClinicManagement.API.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwt;

    public TokenService(IOptions<JwtSettings> jwt) => _jwt = jwt.Value;

    public (string Token, DateTime ExpiresAt) GenerateToken(NguoiDung nguoiDung)
    {
        var roleCode = nguoiDung.VaiTro?.Code ?? string.Empty;
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpireMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, nguoiDung.Id.ToString()),
            new(ClaimTypes.NameIdentifier, nguoiDung.Id.ToString()),
            new(ClaimTypes.Name, nguoiDung.TenDangNhap),
            new("hoTen", nguoiDung.HoTen),
            new(ClaimTypes.Role, roleCode),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

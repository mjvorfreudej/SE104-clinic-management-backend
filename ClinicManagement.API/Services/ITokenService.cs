using ClinicManagement.API.Models;

namespace ClinicManagement.API.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(NguoiDung nguoiDung);
}

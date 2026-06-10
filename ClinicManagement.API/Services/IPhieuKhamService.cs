using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Services;

public interface IPhieuKhamService
{
    Task<PhieuKhamDto> CreateAsync(CreatePhieuKhamRequest request);
    Task<PhieuKhamDto?> GetByMaAsync(string maPhieuKham);
}

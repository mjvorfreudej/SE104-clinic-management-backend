namespace ClinicManagement.API.Common;

/// <summary>
/// Lỗi nghiệp vụ có chủ đích (vi phạm quy định, dữ liệu không hợp lệ...).
/// Middleware sẽ map exception này thành HTTP 400 kèm message thân thiện.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

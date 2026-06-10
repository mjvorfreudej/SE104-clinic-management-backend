using System.Text.Json;
using ClinicManagement.API.Common;
using ClinicManagement.API.DTOs;

namespace ClinicManagement.API.Middleware;

/// <summary>
/// Bắt mọi exception, trả JSON { message } thống nhất.
/// DomainException -> 400; còn lại -> 500 (ẩn chi tiết kỹ thuật khỏi client).
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await WriteAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi không xử lý được tại {Path}", context.Request.Path);
            await WriteAsync(context, StatusCodes.Status500InternalServerError,
                "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
        }
    }

    private static async Task WriteAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new MessageResponse(message),
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await context.Response.WriteAsync(payload);
    }
}

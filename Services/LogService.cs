using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using UserManagementApp.Data;
using UserManagementApp.Models;

namespace UserManagementApp.Services
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogErrorAsync(Exception exception, string? additionalMessage = null, string? userName = null)
        {
            var stackTrace = new StackTrace(exception, true);
            var frame = stackTrace.GetFrame(0);

            var log = new Log
            {
                LogLevel = "Error",
                Message = $"{additionalMessage ?? exception.Message}",
                FileName = frame?.GetFileName() ?? "Unknown",
                MethodName = frame?.GetMethod()?.Name ?? "Unknown",
                LineNumber = frame?.GetFileLineNumber(),
                StackTrace = exception.StackTrace,
                ExceptionType = exception.GetType().Name,
                UserName = userName ?? GetCurrentUser(),
                IpAddress = GetClientIpAddress(),
                RequestPath = GetRequestPath(),
                HttpMethod = GetHttpMethod(),
                CreatedAt = DateTime.Now,
                AdditionalData = JsonSerializer.Serialize(new
                {
                    ExceptionSource = exception.Source,
                    InnerException = exception.InnerException?.Message,
                    AdditionalMessage = additionalMessage
                })
            };

            await SaveLogAsync(log);
        }

        public async Task LogWarningAsync(string message,
            [CallerFilePath] string? fileName = null,
            [CallerMemberName] string? methodName = null)
        {
            var log = new Log
            {
                LogLevel = "Warning",
                Message = message,
                FileName = Path.GetFileName(fileName),
                MethodName = methodName,
                UserName = GetCurrentUser(),
                IpAddress = GetClientIpAddress(),
                RequestPath = GetRequestPath(),
                HttpMethod = GetHttpMethod(),
                CreatedAt = DateTime.Now
            };

            await SaveLogAsync(log);
        }

        public async Task LogInfoAsync(string message,
            [CallerFilePath] string? fileName = null,
            [CallerMemberName] string? methodName = null)
        {
            var log = new Log
            {
                LogLevel = "Info",
                Message = message,
                FileName = Path.GetFileName(fileName),
                MethodName = methodName,
                UserName = GetCurrentUser(),
                IpAddress = GetClientIpAddress(),
                RequestPath = GetRequestPath(),
                HttpMethod = GetHttpMethod(),
                CreatedAt = DateTime.Now
            };

            await SaveLogAsync(log);
        }

        public async Task LogDebugAsync(string message,
            [CallerFilePath] string? fileName = null,
            [CallerMemberName] string? methodName = null)
        {
            var log = new Log
            {
                LogLevel = "Debug",
                Message = message,
                FileName = Path.GetFileName(fileName),
                MethodName = methodName,
                UserName = GetCurrentUser(),
                IpAddress = GetClientIpAddress(),
                RequestPath = GetRequestPath(),
                HttpMethod = GetHttpMethod(),
                CreatedAt = DateTime.Now
            };

            await SaveLogAsync(log);
        }

        public async Task<List<Log>> GetLogsAsync(int pageNumber = 1, int pageSize = 50, string? logLevel = null)
        {
            var query = _context.Logs.AsQueryable();

            if (!string.IsNullOrEmpty(logLevel))
            {
                query = query.Where(l => l.LogLevel == logLevel);
            }

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Log>> GetErrorLogsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Logs.Where(l => l.LogLevel == "Error");

            if (fromDate.HasValue)
                query = query.Where(l => l.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(l => l.CreatedAt <= toDate.Value);

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        private async Task SaveLogAsync(Log log)
        {
            try
            {
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Fallback to file logging if database logging fails
                var logMessage = $"[{DateTime.Now}] Failed to save log to database: {ex.Message}\n" +
                               $"Original log: {log.LogLevel} - {log.Message}\n";

                try
                {
                    await File.AppendAllTextAsync("Logs/fallback.log", logMessage);
                }
                catch
                {
                    // If file logging also fails, write to console as last resort
                    Console.WriteLine(logMessage);
                }
            }
        }

        private string? GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Anonymous";
        }

        private string? GetClientIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        private string? GetRequestPath()
        {
            return _httpContextAccessor.HttpContext?.Request?.Path;
        }

        private string? GetHttpMethod()
        {
            return _httpContextAccessor.HttpContext?.Request?.Method;
        }
    }
}
using UserManagementApp.Models;

namespace UserManagementApp.Services
{
    public interface ILogService
    {
        Task LogErrorAsync(Exception exception, string? additionalMessage = null, string? userName = null);
        Task LogWarningAsync(string message, string? fileName = null, string? methodName = null);
        Task LogInfoAsync(string message, string? fileName = null, string? methodName = null);
        Task LogDebugAsync(string message, string? fileName = null, string? methodName = null);
        Task<List<Log>> GetLogsAsync(int pageNumber = 1, int pageSize = 50, string? logLevel = null);
        Task<List<Log>> GetErrorLogsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}
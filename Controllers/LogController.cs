using Microsoft.AspNetCore.Mvc;
using UserManagementApp.Services;

namespace UserManagementApp.Controllers
{
    public class LogController : Controller
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        public async Task<IActionResult> Index(string? logLevel = null, int page = 1)
        {
            try
            {
                var logs = await _logService.GetLogsAsync(page, 50, logLevel);
                ViewBag.CurrentLogLevel = logLevel;
                ViewBag.CurrentPage = page;
                return View(logs);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, "Error loading logs page");
                return View(new List<UserManagementApp.Models.Log>());
            }
        }

        public async Task<IActionResult> Errors(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var errorLogs = await _logService.GetErrorLogsAsync(fromDate, toDate);
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
                return View(errorLogs);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, "Error loading error logs page");
                return View(new List<UserManagementApp.Models.Log>());
            }
        }
    }
}
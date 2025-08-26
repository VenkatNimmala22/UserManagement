using System.ComponentModel.DataAnnotations;

namespace UserManagementApp.Models
{
    public class Log
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string LogLevel { get; set; } // Error, Warning, Info, Debug

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        [StringLength(200)]
        public string? FileName { get; set; }

        [StringLength(100)]
        public string? MethodName { get; set; }

        public int? LineNumber { get; set; }

        public string? StackTrace { get; set; }

        [StringLength(50)]
        public string? ExceptionType { get; set; }

        [StringLength(200)]
        public string? UserName { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? RequestPath { get; set; }

        [StringLength(10)]
        public string? HttpMethod { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? AdditionalData { get; set; } // JSON for extra info
    }
}
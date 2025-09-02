using System.ComponentModel.DataAnnotations;

namespace UserManagementApp.Models
{
    public class Login
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; } // Store hashed in production
    }
}
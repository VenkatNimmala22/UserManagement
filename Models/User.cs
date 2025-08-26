using System.ComponentModel.DataAnnotations;

namespace UserManagementApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        [StringLength(15)]
        public string? Phone { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
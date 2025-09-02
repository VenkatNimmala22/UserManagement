using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using UserManagementApp.Services;

namespace UserManagementApp.Validation
{
    public class NoIntegersAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var name = value.ToString();
            if (Regex.IsMatch(name, @"\d"))
            {
                // Get the LogService from the validation context
                var logService = (ILogService)validationContext.GetService(typeof(ILogService))!;
                
                // Log the validation error
                logService.LogErrorAsync(
                    new Exception("Name contains integers"), 
                    $"Validation Error: Name '{name}' contains integers which is not allowed",
                    "System"
                ).Wait();

                return new ValidationResult("Full Name cannot contain numbers");
            }

            return ValidationResult.Success;
        }
    }
}
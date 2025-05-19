using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Auth
{
    public class ReactivateAccountDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [DefaultValue("user@example.com")]
        public string Email { get; set; }
    }
}

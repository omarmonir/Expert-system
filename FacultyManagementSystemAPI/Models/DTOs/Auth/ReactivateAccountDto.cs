using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Auth
{
    public class ReactivateAccountDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

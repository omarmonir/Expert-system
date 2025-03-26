using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Auth
{
    public class DeactivateAccountDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    
    }
}

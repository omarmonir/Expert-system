using FacultyManagementSystemAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Data
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string UserType { get; set; }

        public int? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student? Student { get; set; }

        [ForeignKey(nameof(ProfessorId))]
        public Professor? Professor { get; set; }
        public int? ProfessorId { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public string? LastLoginIp { get; set; }
        public string? LastLoginDevice { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? DeactivationDate { get; set; }
    }
}

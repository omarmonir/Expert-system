namespace FacultyManagementSystemAPI.Models.DTOs.professors
{
    public class ProfessorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime JoinDate { get; set; }
        public string Position { get; set; }
        public string ImagePath { get; set; }
        public string DepartmentName { get; set; }
    }
}

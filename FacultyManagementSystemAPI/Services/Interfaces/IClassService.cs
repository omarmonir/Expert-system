using FacultyManagementSystemAPI.Models.DTOs.Classes;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IClassService
    {
        Task AssignCourseToProfessorAsync(int courseId, string professorName);
    }
}

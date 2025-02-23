using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;

        public ClassService(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task AssignCourseToProfessorAsync(int courseId, string professorName)
        {
            var professor = await _classRepository.GetProfessorByNameAsync(professorName) ??
                throw new KeyNotFoundException($"الأستاذ '{professorName}' غير موجود.");

            var course = await _classRepository.GetCourseByIdAsync(courseId) ??
                throw new KeyNotFoundException($"الكورس بالرقم التعريفي {courseId} غير موجود.");

            var existingClass = await _classRepository.GetClassByProfessorNameAsync(professorName) ??
                throw new KeyNotFoundException($"لا يوجد صف دراسي مرتبط بالأستاذ '{professorName}'.");

            bool isAlreadyAssigned = await _classRepository.IsCourseAlreadyAssignedAsync(courseId, professor.Id);
            if (isAlreadyAssigned)
            {
                throw new InvalidOperationException($"الكورس '{course.Name}' تم تعيينه بالفعل للأستاذ '{professorName}'.");
            }

            existingClass.CourseId = courseId;

            await _classRepository.UpdateClassAsync(existingClass);
        }


    }
}

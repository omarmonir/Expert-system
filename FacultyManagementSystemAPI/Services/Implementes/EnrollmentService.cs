using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository) : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository = enrollmentRepository;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly ICourseRepository _courseRepository = courseRepository;

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("رقم المعرف يجب أن يكون رقمًا موجبًا.");

            var existingEnrollment = await _enrollmentRepository.GetByIdAsync(id) ??
                throw new KeyNotFoundException($"لم يتم العثور على تسجيل بالمعرف {id} لحذفه.");

            await _enrollmentRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<EnrollmentDto>> GetAllIncludeStudentNameCourseNameAsync()
        {
            var enrollmentListDto = await _enrollmentRepository.GetAllIncludeStudentNameCourseNameAsync();

            if (!enrollmentListDto.Any() || enrollmentListDto == null)
                throw new KeyNotFoundException("لم يتم العثور على أي سجلات تسجيل.");

            return enrollmentListDto;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByCourseIdAsync(int courseId)
        {
            if (courseId <= 0)
                throw new ArgumentException("رقم المقرر يجب أن يكون رقمًا موجبًا.");

            var courseExists = await _courseRepository.GetByIdAsync(courseId) ??
                throw new KeyNotFoundException($"لم يتم العثور على مقرر بالمعرف {courseId}.");

            var enrollmentListDto = await _enrollmentRepository.GetByCourseIdAsync(courseId);
            if (!enrollmentListDto.Any() || enrollmentListDto == null)
                throw new KeyNotFoundException($"لم يتم العثور على أي سجلات تسجيل لهذا المقرر.");

            return enrollmentListDto;
        }

        public async Task<EnrollmentDto> GetByIdIncludeStudentNameCourseNameAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("رقم المعرف يجب أن يكون رقمًا موجبًا.");

            var enrollmentDto = await _enrollmentRepository.GetByIdIncludeStudentNameCourseNameAsync(id) ??
                throw new KeyNotFoundException($"لم يتم العثور على تسجيل بالمعرف {id}.");
            return enrollmentDto;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetBySemesterAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("الاسم لا يمكن أن يكون فارغًا.");

            var enrollmentListDto = await _enrollmentRepository.GetBySemesterAsync(name);

            if (!enrollmentListDto.Any() || enrollmentListDto == null)
                throw new KeyNotFoundException("لم يتم العثور على أي سجلات تسجيل بهذا فى هذا الفصل.");

            return enrollmentListDto;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByStudentIdAsync(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("رقم الطالب يجب أن يكون رقمًا موجبًا.");

            var studentExists = await _studentRepository.GetByIdAsync(studentId) ??
                throw new KeyNotFoundException($"لم يتم العثور على طالب بالمعرف {studentId}.");

            var enrollmentListDto = await _enrollmentRepository.GetByStudentIdAsync(studentId);
            if (!enrollmentListDto.Any() || enrollmentListDto == null)
                throw new KeyNotFoundException($"لم يتم العثور على أي سجلات تسجيل لهذا الطالب.");

            return enrollmentListDto;
        }

        public async Task<int> GetEnrollmentCountAsync()
        {
            int count = await _enrollmentRepository.CountAsync();
            if (count == 0)
                throw new Exception("لا يوجد تسجيلات.");

            return count;
        }

        public async Task<int> GetCanceledEnrollmentCountAsync()
        {
            int count = await _enrollmentRepository.CountDeletedAsync();
            if (count == 0)
                throw new Exception("لا يوجد تسجيلات محذوفة");

            return count;
        }

    }
}

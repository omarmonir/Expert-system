using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs;
using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository, IMapper mapper) : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository = enrollmentRepository;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly ICourseRepository _courseRepository = courseRepository;
        private readonly IMapper _mapper = mapper;


        public async Task AddAsync(CreateEnrollmentDto enrollmentDto)
        {
            if (enrollmentDto == null)
                throw new ArgumentNullException("البيانات المدخلة لا يمكن أن تكون فارغة.");

            // التأكد من وجود الطالب
            var student = await _enrollmentRepository.GetStudentByIdAsync(enrollmentDto.StudentId)
                ?? throw new KeyNotFoundException($"الطالب غير موجود.");

            // التأكد من وجود الكورس
            var course = await _enrollmentRepository.GetCourseByIdAsync(enrollmentDto.CourseId)
                ?? throw new KeyNotFoundException($"الكورس غير موجود.");

            // التحقق من عدم تكرار التسجيل
            if (await _enrollmentRepository.ExistsAsync(student.Id, course.Id))
                throw new Exception("هذا الطالب مسجل بالفعل في هذا الكورس.");

            var enrollment = _mapper.Map<Enrollment>(enrollmentDto);

            await _enrollmentRepository.AddAsync(enrollment);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("رقم المعرف يجب أن يكون رقمًا موجبًا.");

            var existingEnrollment = await _enrollmentRepository.GetByIdAsync(id) ??
                throw new KeyNotFoundException($"لم يتم العثور على تسجيل بالمعرف {id} لحذفه.");

            await _enrollmentRepository.DeleteAsync(id);
        }

        //public async Task<IEnumerable<EnrollmentDto>> GetAllIncludeStudentNameCourseNameAsync()
        //{
        //    var enrollmentListDto = await _enrollmentRepository.GetAllIncludeStudentNameCourseNameAsync();


        //    if (!enrollmentListDto.Any() || enrollmentListDto == null)
        //        throw new KeyNotFoundException("لم يتم العثور على أي سجلات تسجيل.");

        //    return enrollmentListDto;
        //}
        public async Task<IEnumerable<EnrollmentDto>> GetAllIncludeStudentNameCourseNameAsync(int pageNumber)
        {
            var data = await _enrollmentRepository.GetAllIncludeStudentNameCourseNameAsync(pageNumber);

            if (!data.Any())
                throw new KeyNotFoundException("لم يتم العثور على أي سجلات تسجيل.");

            return data;
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



        public async Task UpdateStudentGradeAsync(int studentId, int courseId, decimal newGrade)
        {
            // التحقق من صحة المدخلات
            if (studentId <= 0 || courseId <= 0)
            {
                throw new ArgumentException("يجب أن يكون معرف الطالب والكورس رقمًا موجبًا.");
            }

            if (newGrade < 0 || newGrade > 90)
            {
                throw new ArgumentException("يجب أن تكون الدرجة بين 0 و 90.");
            }

            // البحث عن التسجيل
            var enrollment = await _enrollmentRepository.GetByStudentAndCourseIdAsync(studentId, courseId) ?? throw new Exception("لم يتم العثور على التسجيل المطلوب.");
            if (enrollment.Grade == newGrade)
                throw new Exception("الدرجة موجوده بالفعل");
            // تحديث الدرجة
            enrollment.Grade = newGrade;

            // حفظ التغييرات في قاعدة البيانات
            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        public async Task UpdateStudentExam1GradeAsync(int studentId, int courseId, decimal newGrade)
        {
            // التحقق من صحة المدخلات
            if (studentId <= 0 || courseId <= 0)
            {
                throw new ArgumentException("يجب أن يكون معرف الطالب والكورس رقمًا موجبًا.");
            }

            if (newGrade < 0 || newGrade > 30)
            {
                throw new ArgumentException("يجب أن تكون الدرجة بين 0 و 30.");
            }

            // البحث عن التسجيل
            var enrollment = await _enrollmentRepository.GetByStudentAndCourseIdAsync(studentId, courseId) ??
                throw new Exception("لم يتم العثور على التسجيل المطلوب.");
            if (enrollment.Exam1Grade == newGrade)
                throw new Exception("الدرجة موجوده بالفعل");
            // تحديث الدرجة
            enrollment.Exam1Grade = newGrade;

            // حفظ التغييرات في قاعدة البيانات
            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        public async Task UpdateStudentExam2GradeAsync(int studentId, int courseId, decimal newGrade)
        {

            // التحقق من صحة المدخلات
            if (studentId <= 0 || courseId <= 0)
            {
                throw new ArgumentException("يجب أن يكون معرف الطالب والكورس رقمًا موجبًا.");
            }

            if (newGrade < 0 || newGrade > 30)
            {
                throw new ArgumentException("يجب أن تكون الدرجة بين 0 و 30.");
            }

            // البحث عن التسجيل
            var enrollment = await _enrollmentRepository.GetByStudentAndCourseIdAsync(studentId, courseId) ?? throw new Exception("لم يتم العثور على التسجيل المطلوب.");
            if (enrollment.Exam2Grade == newGrade)
                throw new Exception("الدرجة موجوده بالفعل");
            // تحديث الدرجة
            enrollment.Exam2Grade = newGrade;

            // حفظ التغييرات في قاعدة البيانات
            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        public async Task<int> GetAllEnrollmentStudentsCountAsync()
        {
            var count = await _enrollmentRepository.GetAllEnrollmentStudentsCountAsync();
            if (count == 0)
                throw new Exception("لا يوجد تسجيلات.");

            return count;
        }

        public async Task<int> GetAllWaitEnrollmentStudentsCountAsync()
        {
            var count = await _enrollmentRepository.GetAllWaitEnrollmentStudentsCountAsync();
            if (count == 0)
                throw new Exception("لا يوجد تسجيلات.");

            return count;
        }

        public async Task<double> GetSuccessPercentageAsync()
        {
            int totalEnrollments = await _enrollmentRepository.CountAsync();
            if (totalEnrollments == 0)
                throw new Exception("لا يوجد تسجيلات.");

            int completedEnrollments = await _enrollmentRepository.GetCompletedEnrollmentCountAsync();
            double successPercentage = ((double)completedEnrollments / totalEnrollments) * 100;

            return Math.Round(successPercentage, 2);
        }

        public async Task<IEnumerable<string>> GetAllEnrollmentsStatusesAsync()
        {
            var status = await _enrollmentRepository.GetAllEnrollmentsStatusesAsync();

            if (status == null || !status.Any())
                throw new Exception("لا يوجد أي حالات للطلاب");

            return status;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetFilteredEnrollmentsAsync(string? studentName, string? courseName, string? enrollmentStatus, string? semester)
        {
            var enrollments = await _enrollmentRepository.GetFilteredEnrollmentsAsync(studentName, courseName, enrollmentStatus, semester);

            if (!enrollments.Any())
                throw new KeyNotFoundException("لا يوجد تسجيلات مطابقة للمعايير المحددة.");

            return enrollments;
        }

        public async Task<IEnumerable<string>> GetAllEnrollmentsSemsterAsync()
        {
            var status = await _enrollmentRepository.GetAllEnrollmentsSemsterAsync();

            if (status == null || !status.Any())
                throw new Exception("لا يوجد أي فصل");

            return status;
        }
    }
}

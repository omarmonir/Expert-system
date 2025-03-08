using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Implementes;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class CourseService(ICourseRepository courseRepository, IMapper mapper) : ICourseService
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICourseRepository _courseRepository = courseRepository;

        public async Task CreateCourseAsync(CreateCourseDto createCourseDto)
        {
            try
            {
                if (createCourseDto == null)
                    throw new ArgumentNullException("البيانات المدخلة لا يمكن أن تكون فارغة.");

                if (await _courseRepository.CourseExistsAsync(createCourseDto.Name))
                    throw new Exception("المقرر موجود بالفعل.");

                if (!await _courseRepository.CourseExistsAsync(createCourseDto.PreCourseId))
                {
                    throw new KeyNotFoundException("لم يتم العثور على المقرر");
                }

                var course = _mapper.Map<Course>(createCourseDto);
                await _courseRepository.AddAsync(course);
              
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"خطأ في قاعدة البيانات: {dbEx.InnerException?.Message}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"حدث خطأ أثناء إضافة الحضور: {ex.Message}", ex);
            }
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"لم يتم العثور على المقرر برقم {id}.");
            await _courseRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CourseDto>> GetAllWithPreCourseNameAsync()
        {
            var coursesDto = await _courseRepository.GetAllWithPreCourseNameAsync();
            if (coursesDto == null || !coursesDto.Any())
                throw new KeyNotFoundException("لم يتم العثور على أي مقررات.");
            return coursesDto;
        }

        public async Task<CourseDto> GetByIdWithPreCourseNameAsync(int id)
        {
            var courseDto = await _courseRepository.GetByIdWithPreCourseNameAsync(id)
                ?? throw new KeyNotFoundException($"لم يتم العثور على المقرر برقم {id}.");
            return courseDto;
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByDepartmentIdWithPreCourseNameAsync(int departmentId)
        {
            var coursesDto = await _courseRepository.GetCoursesByDepartmentIdWithPreCourseNameAsync(departmentId);
            if (coursesDto == null || !coursesDto.Any())
                throw new Exception("لا توجد مقررات لهذا القسم.");
            return coursesDto;
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int professorId)
        {
            var coursesDto = await _courseRepository.GetCoursesByProfessorIdWithPreCourseNameAsync(professorId);
            if (coursesDto == null || !coursesDto.Any())
                throw new Exception("لا توجد مقررات لهذا الدكتور.");
            return coursesDto;
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesBySemesterWithPreCourseNameAsync(byte semester)
        {
            var coursesDto = await _courseRepository.GetCoursesBySemesterWithPreCourseNameAsync(semester);
            if (coursesDto == null || !coursesDto.Any())
                throw new Exception("لا توجد مقررات لهذا الترم.");
            return coursesDto;
        }

        public async Task<IEnumerable<CourseDto>> SearchCoursesWithPreCourseNameAsync(string searchTerm)
        {
            var coursesDto = await _courseRepository.SearchCoursesWithPreCourseNameAsync(searchTerm);
            if (coursesDto == null || !coursesDto.Any())
                throw new Exception($"{searchTerm} غير موجود.");
            return coursesDto;
        }

        public async Task UpdateCourseAsync(int id, UpdateCourseDto updateCourseDto)
        {
            if (updateCourseDto == null)
                throw new ArgumentNullException("البيانات المدخلة لا يمكن أن تكون فارغة.");

            var course = await _courseRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"لم يتم العثور على المقرر برقم {id}.");

            //if (await _courseRepository.CourseExistsAsync(updateCourseDto.Name))
            //    throw new Exception("المقرر موجود بالفعل.");

            var courseUpdate = _mapper.Map(updateCourseDto, course);
            await _courseRepository.UpdateAsync(id, courseUpdate);
        }

        public async Task<int> GetCourseCountAsync()
        {
            int count = await _courseRepository.CountAsync();
            if (count == 0)
                throw new Exception("لا يوجد كورسات.");

            return count;
        }

        public async Task<int> GetCourseCountByStatusAsync()
        {
            int count = await _courseRepository.CountByStatusAsync();
            if (count == 0)
                throw new Exception("لا يوجد كورسات متاحة. ");

            return count;
        }

        public async Task<IEnumerable<string>> GetAllPreRequisiteCoursesAsync()
        {
            var preRequisiteCourses = await _courseRepository.GetAllPreRequisiteCoursesAsync();

            if (preRequisiteCourses == null || !preRequisiteCourses.Any())
            {
                throw new Exception("لم يتم العثور على أي مقررات دراسية لها متطلبات سابقة.");
            }

            return preRequisiteCourses;
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByStudentIdAsync(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("رقم الطالب غير صالح.");

            var courses = await _courseRepository.GetCoursesByStudentIdAsync(studentId);

            if (courses == null || !courses.Any())
                throw new InvalidOperationException("هذا الطالب لم يسجل في أي مادة حتى الآن.");

            return courses;
        }
        public async Task<IEnumerable<CourseRegistrationStatsDto>> GetCourseRegistrationStatsByCourseOverTimeAsync(int courseId)
        {
            if (courseId <= 0)
                throw new ArgumentException("يجب أن يكون معرف المقرر رقمًا أكبر من الصفر", nameof(courseId));

            var stats = await _courseRepository.GetCourseRegistrationStatsByCourseOverTimeAsync(courseId);

            if (stats == null || !stats.Any())
                throw new Exception($"لم يتم العثور على سجلات للتسجيل أو الإلغاء للمقرر برقم {courseId}.");

            return stats;

        }

        public async Task<IEnumerable<string>> GetAllCoursesStatusesAsync()
        {
            var status = await _courseRepository.GetAllCoursesStatusesAsync();

            if (status == null || !status.Any())
                throw new Exception("لا يوجد أي حالات للطلاب");

            return status;
        }


        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف الكورس رقمًا موجبا");

            var student = await _courseRepository.GetByIdAsync(id) ??
                throw new KeyNotFoundException("لم يتم العثور على الكورس"); ;



            // حذف الكورس من قاعدة البيانات
            await _courseRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<string>> GetAllCoursesNameAsync()
        {
            var names = await _courseRepository.GetAllCoursesNameAsync();

            if (names == null || !names.Any())
                throw new Exception("لا يوجد أي كورسات");

            return names;
        }

        public async Task<IEnumerable<FilterCourseDto>> GetFilteredCoursesAsync(string? courseName, string? departmentName, string? courseStatus)
        {
            var courses = await _courseRepository.GetFilteredCoursesAsync(courseName, departmentName, courseStatus);

            if (!courses.Any())
                throw new KeyNotFoundException("لا يوجد كورسات مطابقة للمعايير المحددة.");

            return courses;
        }
    }
}

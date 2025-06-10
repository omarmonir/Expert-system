using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class CourseService(ICourseRepository courseRepository, IMapper mapper) : ICourseService
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICourseRepository _courseRepository = courseRepository;

        public async Task CreateCourseAsync(CreateCourseDto dto)
        {
            var department = await _courseRepository.GetDepartmentByNameAsync(dto.DepartmentName);
            if (department == null)
                throw new Exception("القسم المحدد غير موجود.");

            var divisions = await _courseRepository.GetDivisionsByNamesAsync(dto.DivisionNames);
            var missingDivisions = dto.DivisionNames.Except(divisions.Select(d => d.Name)).ToList();
            if (missingDivisions.Any())
                throw new Exception($"الأقسام التالية غير موجودة: {string.Join(", ", missingDivisions)}");

            var prerequisites = await _courseRepository.GetCoursesByNamesPreAsync(dto.PrerequisiteCourseNames);
            var missingPrereqs = dto.PrerequisiteCourseNames.Except(prerequisites.Select(p => p.Name)).ToList();
            if (missingPrereqs.Any())
                throw new Exception($"المقررات السابقة التالية غير موجودة: {string.Join(", ", missingPrereqs)}");

            var course = new Course
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                Credits = dto.Credits,
                Status = dto.Status,
                Semester = dto.Semester,
                MaxSeats = dto.MaxSeats,
                DepartmentId = department.Id,
                CourseDivisions = divisions
                    .Select(d => new CourseDivision { DivisionId = d.Id }).ToList(),
                Prerequisites = prerequisites
                    .Select(p => new CoursePrerequisite { PrerequisiteCourseId = p.Id }).ToList()
            };

            await _courseRepository.AddCourseAsync(course);
            await _courseRepository.SaveChangesAsync();

        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _courseRepository.GetByIdWithEnrollmentsAsync(id)
                ?? throw new KeyNotFoundException($"لم يتم العثور على المقرر برقم {id}.");

            if (course.Enrollments.Any())
                throw new InvalidOperationException("لا يمكن حذف هذا المقرر لأنه يوجد طلاب مسجلين فيه.");

            if (course.IsPrerequisiteFor.Any())
                throw new InvalidOperationException("لا يمكن حذف الكورس لأنه مستخدم كمقرر تمهيدي لكورسات أخرى.");

            await _courseRepository.DeleteAsync(id);
        }


        public async Task<IEnumerable<CourseDto>> GetAllWithPreCourseNameAsync(int pageNumber)
        {
            var coursesDto = await _courseRepository.GetAllWithPreCourseNameAsync(pageNumber);
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

        //public async Task<IEnumerable<CourseDto>> GetCoursesByDepartmentIdWithPreCourseNameAsync(int departmentId)
        //{
        //    var coursesDto = await _courseRepository.GetCoursesByDepartmentIdWithPreCourseNameAsync(departmentId);
        //    if (coursesDto == null || !coursesDto.Any())
        //        throw new Exception("لا توجد مقررات لهذا القسم.");
        //    return coursesDto;
        //}

        public async Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int professorId)
        {
            var coursesDto = await _courseRepository.GetCoursesByProfessorIdWithPreCourseNameAsync(professorId);
            if (coursesDto == null || !coursesDto.Any())
                throw new Exception("لا توجد مقررات لهذا الدكتور.");
            return coursesDto;
        }

        //public async Task<IEnumerable<CourseDto>> GetCoursesBySemesterWithPreCourseNameAsync(byte semester)
        //{
        //    var coursesDto = await _courseRepository.GetCoursesBySemesterWithPreCourseNameAsync(semester);
        //    if (coursesDto == null || !coursesDto.Any())
        //        throw new Exception("لا توجد مقررات لهذا الترم.");
        //    return coursesDto;
        //}

        //public async Task<IEnumerable<CourseDto>> SearchCoursesWithPreCourseNameAsync(string searchTerm)
        //{
        //    var coursesDto = await _courseRepository.SearchCoursesWithPreCourseNameAsync(searchTerm);
        //    if (coursesDto == null || !coursesDto.Any())
        //        throw new Exception($"{searchTerm} غير موجود.");
        //    return coursesDto;
        //}

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

        //public async Task<int> GetCourseCountByStatusAsync()
        //{
        //    int count = await _courseRepository.CountByStatusAsync();
        //    if (count == 0)
        //        throw new Exception("لا يوجد كورسات متاحة. ");

        //    return count;
        //}

        //public async Task<IEnumerable<string>> GetAllPreRequisiteCoursesAsync()
        //{
        //    var preRequisiteCourses = await _courseRepository.GetAllPreRequisiteCoursesAsync();

        //    if (preRequisiteCourses == null || !preRequisiteCourses.Any())
        //    {
        //        throw new Exception("لم يتم العثور على أي مقررات دراسية لها متطلبات سابقة.");
        //    }

        //    return preRequisiteCourses;
        //}

        public async Task<IEnumerable<CourseDto>> GetCoursesByStudentIdAsync(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("رقم الطالب غير صالح.");

            var courses = await _courseRepository.GetCoursesByStudentIdAsync(studentId);

            if (courses == null || !courses.Any())
                throw new InvalidOperationException("هذا الطالب لم يسجل في أي مادة حتى الآن.");

            return courses;
        }
        //public async Task<IEnumerable<CourseRegistrationStatsDto>> GetCourseRegistrationStatsByCourseOverTimeAsync(int courseId)
        //{
        //    if (courseId <= 0)
        //        throw new ArgumentException("يجب أن يكون معرف المقرر رقمًا أكبر من الصفر", nameof(courseId));

        //    var stats = await _courseRepository.GetCourseRegistrationStatsByCourseOverTimeAsync(courseId);

        //    if (stats == null || !stats.Any())
        //        throw new Exception($"لم يتم العثور على سجلات للتسجيل أو الإلغاء للمقرر برقم {courseId}.");

        //    return stats;

        //}

        //public async Task<IEnumerable<string>> GetAllCoursesStatusesAsync()
        //{
        //    var status = await _courseRepository.GetAllCoursesStatusesAsync();

        //    if (status == null || !status.Any())
        //        throw new Exception("لا يوجد أي حالات للطلاب");

        //    return status;
        //}


        //public async Task DeleteAsync(int id)
        //{
        //    if (id <= 0)
        //        throw new ArgumentException("يجب أن يكون معرف الكورس رقمًا موجبا");

        //    var student = await _courseRepository.GetByIdAsync(id) ??
        //        throw new KeyNotFoundException("لم يتم العثور على الكورس"); ;



        //    // حذف الكورس من قاعدة البيانات
        //    await _courseRepository.DeleteAsync(id);
        //}

        public async Task<IEnumerable<string>> GetAllCoursesNameAsync()
        {
            var names = await _courseRepository.GetAllCoursesNameAsync();

            if (names == null || !names.Any())
                throw new Exception("لا يوجد أي كورسات");

            return names;
        }

        public async Task<IEnumerable<FilterCourseDto>> GetFilteredCoursesAsync(string? courseName, string? departmentName, string? courseStatus, string? divisionName)
        {
            var courses = await _courseRepository.GetFilteredCoursesAsync(courseName, departmentName, courseStatus, divisionName);

            if (!courses.Any())
                throw new KeyNotFoundException("لا يوجد كورسات مطابقة للمعايير المحددة.");

            return courses;
        }

        public async Task<CourseStatisticsDto> GetCourseStatisticsAsync(int courseId)
        {
            bool courseExists = await _courseRepository.CourseExistsAsync(courseId);
            if (!courseExists)
            {
                throw new KeyNotFoundException($"لا توجد مادة بالمعرف {courseId}");
            }

            // جلب الإحصائيات من الريبوزيتوري
            var statistics = await _courseRepository.GetCourseStatisticsAsync(courseId);
            return statistics;
        }

        //public async Task<int> CountActiveCourseAsync()
        //{
        //    int count = await _courseRepository.CountActiveCourseAsync();
        //    if (count == 0)
        //        throw new Exception("لا يوجد مقررات نشطه");

        //    return count;
        //}
        //public async Task<IEnumerable<CourseDto>> SearchCoursesWithCourseNameAndStatusAsync(string searchTerm, string status)
        //{
        //    var coursesDto = await _courseRepository.SearchCoursesWithCourseNameAndStatusAsync(searchTerm, status);
        //    if (coursesDto == null || !coursesDto.Any())
        //        throw new Exception($"{searchTerm} غير موجود.");
        //    return coursesDto;
        //}

    }
}

using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //public class StudentController(IStudentService studentService) : ControllerBase
    public class StudentController(IStudentService studentService, ICourseService courseService, IEnrollmentService enrollmentService) : ControllerBase
    {
        private readonly IStudentService _studentService = studentService;
        //private readonly ICourseService _courseService = courseService;
        //private readonly IEnrollmentService _enrollmentService = enrollmentService;

        //[Authorize(Roles = ConstantRoles.Admin)]
        //[Authorize(Roles = ConstantRoles.SuperAdmin)]
        [HttpGet("AllStudents")]
        public async Task<IActionResult> GetAllWithDepartmentName([FromQuery] int pageNumber = 1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var studentsDto = await _studentService.GetAllWithDepartmentNameAsync(pageNumber);
                return Ok(studentsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("StudentById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var studentDto = await _studentService.GetByIdWithDepartmentNameAsync(id);
                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("StudentByName/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var studentsDto = await _studentService.GetByNameWithDepartmentNameAsync(name);
                return Ok(studentsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GradesOFStudentById/{id:int}")]
        public async Task<IActionResult> GetGradesOFStudentById(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var studentDto = await _studentService.GetByIdWithHisGradeAsync(id);
                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("AddStudent")]
        //[Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Add([FromForm] CreateStudentDto createStudentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _studentService.AddAsync(createStudentDto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddMultipleStudents")]
        //[Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> AddMultiple([FromBody] List<CreateStudentDto> students)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                List<string> results = new List<string>();
                foreach (var student in students)
                {
                    try
                    {
                        await _studentService.AddMultipleAsync(student);
                        results.Add($"تم إضافة الطالب {student.Name} بنجاح");
                    }
                    catch (Exception ex)
                    {
                        results.Add($"فشل إضافة الطالب {student.Name}: {ex.Message}");
                    }
                }

                return Ok(new { Message = "تمت معالجة إضافة الطلاب", Results = results });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateStudent/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateStudentDto updateStudentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _studentService.UpdateAsync(id, updateStudentDto);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteStudent/{id:int}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _studentService.DeleteAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountOfStudentsByDepartmentId/{departmentId}")]
        public async Task<IActionResult> GetStudentCountByDepartment(int departmentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var studentCounts = await _studentService.GetStudentCountByDepartmentAsync(departmentId);

                return Ok(studentCounts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("UnenrolledStudents")]
        public async Task<IActionResult> GetUnenrolledStudents()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var students = await _studentService.GetUnenrolledStudentsAsync();
                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("UnenrolledStudentsByDepartmentId/{departmentId}")]
        public async Task<IActionResult> GetUnenrolledStudentsByDepartment(int departmentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var students = await _studentService.GetUnenrolledStudentsByDepartmentAsync(departmentId);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("UnenrolledBySemester/{semester}")]
        public async Task<IActionResult> GetUnenrolledStudentsBySemester(byte semester)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var students = await _studentService.GetUnenrolledStudentsBySemesterAsync(semester);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FilterByDate")]
        public async Task<IActionResult> GetEnrollmentsByDateRange([FromQuery] DateTime? minDate, [FromQuery] DateTime? maxDate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var enrollments = await _studentService.GetEnrollmentsByDateRangeAsync(minDate, maxDate);
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Filtering")]
        public async Task<IActionResult> GetFilteredStudents([FromQuery] StudentFilterDto filter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var students = await _studentService.GetFilteredStudentsAsync(filter);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("StudentsByDepartmentId/{departmentId}")]
        public async Task<IActionResult> GetStudentsByDepartment(int departmentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var students = await _studentService.GetAllByDepartmentIdAsync(departmentId);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("StudentsByDepartmentName/{departmentName}")]
        public async Task<IActionResult> GetStudentsByDepartmentName(string departmentName)
        {
            try
            {
                var students = await _studentService.GetStudentsByDepartmentNameAsync(departmentName);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("CountOfStudentsAndCourses")]
        //public async Task<IActionResult> GetStudentCount()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    try
        //    {
        //        var countOfAllStudents = await _studentService.GetStudentCountAsync();
        //        var countOfAllCourses = await _courseService.GetCourseCountAsync();

        //        int countOfStudents = await _studentService.GetStudentCountAsync();
        //        int countOfEnrollments = await _enrollmentService.GetAllEnrollmentStudentsCountAsync();

        //        var total = ((decimal)countOfEnrollments / countOfStudents) * 100;
        //        total = Math.Round(total, 2);


        //        return Ok(new
        //        {
        //            countOfStudents = countOfAllStudents,
        //            countOfCourses = countOfAllCourses,
        //            enrollmentPercentage = total,

        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet("CountOfAllEnrollmentStudents")]
        public async Task<IActionResult> GetEnrollmentStudents()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _studentService.GetAllEnrollmentStudentsCountAsync();
                return Ok(new { countOfStudents = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("StudentStatistics")]
        public async Task<IActionResult> GetStudentEnrollmentStats()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var stats = await _studentService.GetStudentEnrollmentStatsAsync();
                return Ok(new
                {
                    totalStudents = stats.totalStudents,
                    enrollmentRatio = stats.enrollmentRatio
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountOfCanceledEnrollmentStudents")]
        public async Task<IActionResult> GetCanceledEnrollmentStudents()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _studentService.CountCanceledEnrolledStudentsAsync();
                return Ok(new { countOfStudents = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountEnrollmentCoursesByStudentId/{studentId}")]
        public async Task<IActionResult> GetCanceledEnrollmentStudents(int studentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _studentService.CountEnrollmentCoursesByStudentIdAsync(studentId);
                return Ok(new { countOfEnrollments = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("CountCompletedCoursesCountStudentId/{studentId}")]
        public async Task<IActionResult> CountCompletedCoursesCountStudentId(int studentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _studentService.CountCompletedCoursesCountStudentIdAsync(studentId);
                return Ok(new { countOfCompletedCourses = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountStudentByCourseId/{courseId}")]
        public async Task<IActionResult> CountStudentByCourseId(int courseId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _studentService.CountStudentsByCourseIdAsync(courseId);
                return Ok(new { countOfStudents = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FilterByStudentNameAndDepartmentNameAndStudentStatusAndDivisionName")]
        public async Task<IActionResult> GetStudentsByDepartmentAndName(
         [FromQuery] string? departmentName,
         [FromQuery] string? studentName,
         [FromQuery] string? studentStatus,
         [FromQuery] string? divisionName)
        {
            try
            {
                var students = await _studentService
                    .GetStudentsByDepartmentAndNameAsync(departmentName, studentName, studentStatus, divisionName);

                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("AllStudentStatuses")]
        public async Task<IActionResult> GetAllStudentStatuses()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var status = await _studentService.GetAllStudentStatusesAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 
        
        [HttpGet("AllStudentNames")]
        public async Task<IActionResult> GetAllStudentNames()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var status = await _studentService.GetAllStudentNamesAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AllStudentLevels")]
        public async Task<IActionResult> GetAllStudentLevels()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var studentLevel = await _studentService.GetAllStudentLevelsAsync();
                return Ok(studentLevel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("StudentsWithExamGradesByCourseId/{courseId}")]
        public async Task<IActionResult> GetStudentsWithExamGradesByCourseId(int courseId)
        {
            var students = await _studentService.GetStudentsWithExamGradesByCourseIdAsync(courseId);
            if (students == null || students.Count() == 0)
            {
                return NotFound($"لم يتم العثور على طلاب مسجلين في الكورس برقم {courseId}.");
            }
            return Ok(students);
        }

        [HttpGet("AllStudentGender")]
        public async Task<IActionResult> GetAllStudentGender()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var genders = await _studentService.GetAllStudentGenderAsync();
                return Ok(genders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updateStudentStatus/{studentId}")]
        public async Task<IActionResult> UpdateProfessorCount(int studentId, string newStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _studentService.UpdateStudentStatusAsync(studentId, newStatus);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetGradesByStudentId/{studentId}")]
        public async Task<IActionResult> GetGradesByStudentId(int studentId)
        {
            try
            {
                var result = await _studentService.GetStudentGradesByStudentIdAsync(studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

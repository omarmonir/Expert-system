using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(IStudentService studentService) : ControllerBase
    {
        private readonly IStudentService _studentService = studentService;

        [HttpGet("AllStudents")]
        public async Task<IActionResult> GetAllWithDepartmentName()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var studentsDto = await _studentService.GetAllWithDepartmentNameAsync();
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

        [HttpGet("CountOfStudents")]
        public async Task<IActionResult> GetStudentCount()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _studentService.GetStudentCountAsync();
                return Ok(new { countOfStudents = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountOfEnrollmentStudents")]
        public async Task<IActionResult> GetEnrollmentStudents()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _studentService.GetEnrolledStudentCountAsync();
                return Ok(new { countOfStudents = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FilterByStudentNameAndDepartmentNameAndStudentStatus")]
        public async Task<IActionResult> GetStudentsByDepartmentAndName([FromQuery] string? departmentName, [FromQuery] string? studentName, [FromQuery] string? studentStatus)
        {
            try
            {
                var students = await _studentService.GetStudentsByDepartmentAndNameAsync(departmentName, studentName, studentStatus);
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
    }
}

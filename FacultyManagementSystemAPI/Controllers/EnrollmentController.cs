using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController(IEnrollmentService enrollmentService, IStudentService studentService) : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService = enrollmentService;
        private readonly IStudentService _studentService = studentService;

        //[HttpGet("AllEnrollments")]
        //public async Task<IActionResult> GetAll()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    try
        //    {
        //        var enrollments = await _enrollmentService.GetAllIncludeStudentNameCourseNameAsync();
        //        return Ok(enrollments);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpGet("AllEnrollments")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _enrollmentService.GetAllIncludeStudentNameCourseNameAsync(pageNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("EnrollmentById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var enrollmentDto = await _enrollmentService.GetByIdIncludeStudentNameCourseNameAsync(id);
                return Ok(enrollmentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddEnrollment")]
        public async Task<IActionResult> AddEnrollment([FromBody] CreateEnrollmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _enrollmentService.AddAsync(dto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("EnrollmentByStudentId/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var enrollmentListDto = await _enrollmentService.GetByStudentIdAsync(studentId);
                return Ok(enrollmentListDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("EnrollmentByCourseId/{courseId}")]
        public async Task<IActionResult> GetByCourseId(int courseId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var enrollmentListDto = await _enrollmentService.GetByCourseIdAsync(courseId);
                return Ok(enrollmentListDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("EnrollmentBySemester/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var enrollmentListDto = await _enrollmentService.GetBySemesterAsync(name);
                return Ok(enrollmentListDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteEnrollmentById/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _enrollmentService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountOfEnrollments")]
        public async Task<IActionResult> GetEnrollmentCount()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _enrollmentService.GetEnrollmentCountAsync();
                return Ok(new { countOfEnrollments = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountOfCanceledEnrollments")]
        public async Task<IActionResult> GetCanceledEnrollmentCount()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _enrollmentService.GetCanceledEnrollmentCountAsync();
                return Ok(new { countOfCanceledEnrollments = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("updateGrade")]
        public async Task<IActionResult> UpdateGrade([FromBody] UpdateGradeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _enrollmentService.UpdateStudentGradeAsync(dto.StudentId, dto.CourseId, dto.NewGrade);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateStudentExam1Grade")]
        public async Task<IActionResult> UpdateStudentExam1Grade([FromBody] UpdateGradeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _enrollmentService.UpdateStudentExam1GradeAsync(dto.StudentId, dto.CourseId, dto.NewGrade);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateStudentExam2Grade")]
        public async Task<IActionResult> UpdateStudentExam2Grade([FromBody] UpdateGradeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _enrollmentService.UpdateStudentExam2GradeAsync(dto.StudentId, dto.CourseId, dto.NewGrade);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AllEnrollmentStudentsCount")]
        public async Task<IActionResult> GetAllEnrollmentStudentsCount()
        {
            try
            {
                int count = await _enrollmentService.GetAllEnrollmentStudentsCountAsync();
                return Ok(new { enrollmentStudentsCount = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("SuccessEnrollmentPercentage")]
        public async Task<IActionResult> GetSuccessPercentage()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                double successPercentage = await _enrollmentService.GetSuccessPercentageAsync();
                return Ok(new { successEnrollmentPercentage = successPercentage });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("EnrollmentPercentage")]
        public async Task<IActionResult> GetSPercentage()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                int countOfStudents = await _studentService.GetStudentCountAsync();
                int countOfEnrollments = await _enrollmentService.GetEnrollmentCountAsync();

                decimal total = (countOfEnrollments / countOfStudents) * 100;
                return Ok(new { successEnrollmentPercentage = total });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AllEnrollmentStatuses")]
        public async Task<IActionResult> GetAllStudentStatuses()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var status = await _enrollmentService.GetAllEnrollmentsStatusesAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AllEnrollmentsSemster")]
        public async Task<IActionResult> GetAllEnrollmentsSemster()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var semster = await _enrollmentService.GetAllEnrollmentsSemsterAsync();
                return Ok(semster);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Filtering")]
        public async Task<IActionResult> GetFilteredEnrollments([FromQuery] string? studentName, [FromQuery] string? courseName, [FromQuery] string? enrollmentStatus, [FromQuery] string? semester)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var enrollments = await _enrollmentService.GetFilteredEnrollmentsAsync(studentName, courseName, enrollmentStatus, semester);
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

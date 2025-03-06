using FacultyManagementSystemAPI.Models.DTOs.Department;
using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Services.Implementes;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController(IEnrollmentService enrollmentService) : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService = enrollmentService;

        [HttpGet("AllEnrollments")]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var enrollments = await _enrollmentService.GetAllIncludeStudentNameCourseNameAsync();
                return Ok(enrollments);
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

    }
}

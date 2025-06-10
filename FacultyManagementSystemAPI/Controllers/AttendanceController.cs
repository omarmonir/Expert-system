using FacultyManagementSystemAPI.Models.DTOs.Attendance;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet("AllAttendances")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1)
        {
            try
            {
                var result = await _attendanceService.GetAllAttendancesAsync(pageNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AttendanceById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _attendanceService.GetAttendanceByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddAttendance")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> Create([FromBody] CreateAttendanceDto createAttendanceDto)
        {
            try
            {
                var professorId = int.Parse(User.FindFirst("ProfessorId").Value); // حسب الكليم

                await _attendanceService.AddAttendanceAsync(createAttendanceDto, professorId);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateAttendance/{id}")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAttendanceDto updateAttendanceDto)
        {
            try
            {
                var professorId = int.Parse(User.FindFirst("ProfessorId").Value); // أو الكليم المستخدم في التوكن

                await _attendanceService.UpdateAttendanceAsync(id, updateAttendanceDto, professorId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("DeleteAttendance/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _attendanceService.DeleteAttendanceAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AttendancesByStudentId/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId, [FromQuery] int pageNumber = 1)
        {
            try
            {
                var result = await _attendanceService.GetAttendancesByStudentIdAsync(studentId, pageNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AttendancesByClassID/{classId}")]
        public async Task<IActionResult> GetByClassId( int classId, [FromQuery] int pageNumber = 1)
        {
            try
            {
                var result = await _attendanceService.GetAttendancesByClassIdAsync(classId, pageNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountOfAttendances")]
        public async Task<IActionResult> GetAttendancesCount()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _attendanceService.CountAttendanceAsync();
                return Ok(new { countOfStudents = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountOfNoAttendances")]
        public async Task<IActionResult> GetNoAttendancesCount()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _attendanceService.CountNoAttendanceAsync();
                return Ok(new { countOfStudents = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("SuccessAttendancePercentage")]
        public async Task<IActionResult> GetSuccessPercentage()
        {
            try
            {
                double successPercentage = await _attendanceService.GetSuccessPercentageAsync();
                return Ok(new { successAttendancePercentage = successPercentage });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

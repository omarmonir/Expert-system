using FacultyManagementSystemAPI.Models.DTOs.Class;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpPut("AssignCourseToProfessor")]
        public async Task<IActionResult> AssignCourseToProfessor([FromBody] AssignClassRequestDto assignClassRequest)
        {
            try
            {
                await _classService.AssignCourseToProfessorAsync(assignClassRequest.CourseId, assignClassRequest.ProfessorName);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

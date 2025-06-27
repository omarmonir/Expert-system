using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Services.Implementes;
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

        [HttpPost("CreateClass")]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassByNameDto createClassDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _classService.CreateClassByNameAsync(createClassDto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateClass/{id}")]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] UpdateClassDto updateClassDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _classService.UpdateClassAsync(id, updateClassDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

        [HttpGet("Classes")]
        public async Task<IActionResult> GetAllClasses([FromQuery] int pageNumber = 1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var classDto = await _classService.GetAllClassesWithProfNameAndCourseNameAsync(pageNumber);
                return Ok(classDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ClassById/{id:int}")]
        public async Task<IActionResult> ClassById(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var courseDto = await _classService.GetClassByIdAsync(id);
                return Ok(courseDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountOfClasses")]
        public async Task<IActionResult> CountOfClasses()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _classService.GetClassCountAsync();
                return Ok(new { countOfClasses = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AllNamesLocations")]
        public async Task<IActionResult> GetAllName()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var names = await _classService.GetAllLocationsNameAsync();
                return Ok(names);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteClass/{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _classService.DeleteClassAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("FilterByDivisionNameAndSemster")]
        public async Task<IActionResult> GetAllClassesWithProfNameAndCourseName(
        [FromQuery] string? level,
        [FromQuery] string? divisionName)
        {
            try
            {
                var students = await _classService
                    .GetAllClassesWithProfNameAndCourseNameAsyncOptimized(divisionName, level);

                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("ProfessorClasse")]
        public async Task<IActionResult> GetMyClasses()
        {
            var professorIdClaim = User.FindFirst("ProfessorId");
            if (professorIdClaim == null)
                return Unauthorized("Invalid token: ProfessorId not found");

            int professorId = int.Parse(professorIdClaim.Value);

            var classes = await _classService.GetProfessorClassesAsync(professorId);

            return Ok(classes);
        }

        [HttpGet("StudentClasses")]
        public async Task<IActionResult> GetClasses()
        {
            var studentIdClaim = User.FindFirst("StudentId");
            if (studentIdClaim == null)
                return Unauthorized("Invalid token: StudentId not found");

            int studentId = int.Parse(studentIdClaim.Value);
            var classes = await _classService.GetStudentClassesAsync(studentId);
            return Ok(classes);
        }

    }
}

using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController(ICourseService courseService) : ControllerBase
    {
        private readonly ICourseService _courseService = courseService;


        [HttpGet("AllCourses")]

        public async Task<IActionResult> GetAllCourses()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var coursesDto = await _courseService.GetAllWithPreCourseNameAsync();
                return Ok(coursesDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       [HttpGet("CourseById/{id:int}")]

        public async Task<IActionResult> GetByIdWithPreCourseName(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var courseDto = await _courseService.GetByIdWithPreCourseNameAsync(id);
                return Ok(courseDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("SearchCourses/{searchTerm}")]
        public async Task<IActionResult> SearchCoursesWithPreCourseName(string searchTerm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var courses = await _courseService.SearchCoursesWithPreCourseNameAsync(searchTerm);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateCourse")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _courseService.CreateCourseAsync(createCourseDto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CoursesBySemester/{semester}")]
        public async Task<IActionResult> GetCoursesBySemesterWithPreCourseName(byte semester)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var courses = await _courseService.GetCoursesBySemesterWithPreCourseNameAsync(semester);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCoursesByProfessorId/{professorId}")]
        public async Task<IActionResult> GetCoursesByProfessorIdWithPreCourseName(int professorId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var courses = await _courseService.GetCoursesByProfessorIdWithPreCourseNameAsync(professorId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCourseRegistrationStatsByCourseOverTime/{courseId}")]
        public async Task<IActionResult> GetCourseRegistrationStatsByCourseOverTime(int courseId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var courses = await _courseService.GetCourseRegistrationStatsByCourseOverTimeAsync(courseId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("CoursesByStudentId/{studentId}")]
        public async Task<IActionResult> GetCoursesByStudentId(int studentId)
        {
            try
            {
                var courses = await _courseService.GetCoursesByStudentIdAsync(studentId);
                return Ok(courses);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ غير متوقع.", details = ex.Message });
            }
        }



        [HttpPut("UpdateCourse/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _courseService.UpdateCourseAsync(id, updateCourseDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteCourse/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return Content(ex.Message);// 404 Not Found
            }
        }


        [HttpGet("CountOfCourses")]
        public async Task<IActionResult> GetCourseCount()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var count = await _courseService.GetCourseCountAsync();
                return Ok(new { countOfCourses = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CountOfAvailableCourses")]
        public async Task<IActionResult> GetCourseCountByStatus()
        {
            var count = await _courseService.GetCourseCountByStatusAsync();
            return Ok(new { countOfCourses = count });
        }

        [HttpGet("AllPreCoursesName")]
        public async Task<IActionResult> GetAllPreCoursesName()
        {
            try
            {
                var preCoursesNames = await _courseService.GetAllPreRequisiteCoursesAsync();
                return Ok(preCoursesNames);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

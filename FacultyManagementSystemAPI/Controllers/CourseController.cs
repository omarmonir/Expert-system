using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Services.Implementes;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController(ICourseService courseService) : ControllerBase
    {
        private readonly ICourseService _courseService = courseService;


        [HttpGet("AllCourses")]
    public async Task<IActionResult> GetAllCourses([FromQuery] int pageNumber = 1)
        {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var coursesDto = await _courseService.GetAllWithPreCourseNameAsync(pageNumber);

            if (coursesDto == null || !coursesDto.Any())
                return NotFound("لم يتم العثور على أي مقررات.");

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

        //    [HttpGet("SearchCourses/{searchTerm}")]
        //    public async Task<IActionResult> SearchCoursesWithPreCourseName(string searchTerm)
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        try
        //        {
        //            var courses = await _courseService.SearchCoursesWithPreCourseNameAsync(searchTerm);
        //            return Ok(courses);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

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


        //    [HttpGet("CoursesBySemester/{semester}")]
        //    public async Task<IActionResult> GetCoursesBySemesterWithPreCourseName(byte semester)
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        try
        //        {
        //            var courses = await _courseService.GetCoursesBySemesterWithPreCourseNameAsync(semester);
        //            return Ok(courses);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

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

        //    [HttpGet("GetCourseRegistrationStatsByCourseOverTime/{courseId}")]
        //    public async Task<IActionResult> GetCourseRegistrationStatsByCourseOverTime(int courseId)
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        try
        //        {
        //            var courses = await _courseService.GetCourseRegistrationStatsByCourseOverTimeAsync(courseId);
        //            return Ok(courses);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        [Authorize(Roles = ConstantRoles.Student)]
        [HttpGet("CoursesByStudentId/{studentId}")]
        public async Task<IActionResult> GetCoursesByStudentId(int studentId)
        {
            try
            {
                var courses = await _courseService.GetCoursesByStudentIdAsync(studentId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _courseService.DeleteCourseAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // 409 Conflict
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // 404 Not Found
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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

        //    [HttpGet("AllCoursesStatuses")]
        //    public async Task<IActionResult> GetAllCoursesStatuses()
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        try
        //        {
        //            var status = await _courseService.GetAllCoursesStatusesAsync();
        //            return Ok(status);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        [HttpGet("AllNamesCourse")]
        public async Task<IActionResult> GetAllName()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var names = await _courseService.GetAllCoursesNameAsync();
                return Ok(names);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        //    [HttpGet("CountOfAvailableCourses")]
        //    public async Task<IActionResult> GetCourseCountByStatus()
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        try
        //        {
        //            var count = await _courseService.GetCourseCountByStatusAsync();
        //            return Ok(new { countOfCourses = count });
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //    [HttpGet("AllPreCoursesName")]
        //    public async Task<IActionResult> GetAllPreCoursesName()
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        try
        //        {
        //            var preCoursesNames = await _courseService.GetAllPreRequisiteCoursesAsync();
        //            return Ok(preCoursesNames);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        [HttpGet("FilterCourses")]
        public async Task<IActionResult> GetFilteredCourses(
                                        [FromQuery] string? courseName,
                                        [FromQuery] string? departmentName,
                                        [FromQuery] string? courseStatus,
                                        [FromQuery] string? divisionName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var courses = await _courseService.GetFilteredCoursesAsync(courseName, departmentName, courseStatus, divisionName);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("CourseStatistics/{courseId}")]
        public async Task<IActionResult> GetCourseStatistics(int courseId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var statistics = await _courseService.GetCourseStatisticsAsync(courseId);
                return Ok(statistics);
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

        //    [HttpGet("CountActiveCourse")]
        //    public async Task<IActionResult> CountActiveCourse()
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        try
        //        {
        //            var count = await _courseService.CountActiveCourseAsync();
        //            return Ok(new { CountActiveCourse = count });
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //    [HttpPost("SearchCoursesWithStatus")]
        //    public async Task<IActionResult> SearchCoursesWithCourseNameAndStatus([FromBody] SearchCoursesRequestDto request)
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        try
        //        {
        //            var courses = await _courseService.SearchCoursesWithCourseNameAndStatusAsync(request.SearchTerm, request.Status);
        //            return Ok(courses);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }
    }


}

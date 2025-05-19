using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class ClassController : ControllerBase
    //{
    //    private readonly IClassService _classService;
    //    public ClassController(IClassService classService)
    //    {
    //        _classService = classService;
    //    }

    //    [HttpPost("CreateClass")]
    //    public async Task<IActionResult> CreateClass([FromBody] CreateClassDto createClassDto)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }

    //            await _classService.CreateClassAsync(createClassDto);
    //            return StatusCode(201);
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //    }

    //    [HttpPut("UpdateClass/{id}")]
    //    public async Task<IActionResult> UpdateClass(int id, [FromForm] UpdateClassDto updateClassDto)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }

    //            await _classService.UpdateClassAsync(id, updateClassDto);
    //            return NoContent();
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //    }

    //    [HttpPut("AssignCourseToProfessor")]
    //    public async Task<IActionResult> AssignCourseToProfessor([FromBody] AssignClassRequestDto assignClassRequest)
    //    {
    //        try
    //        {
    //            await _classService.AssignCourseToProfessorAsync(assignClassRequest.CourseId, assignClassRequest.ProfessorName);
    //            return NoContent();
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //    }

    //    [HttpGet("Classes")]
    //    public async Task<IActionResult> GetAllClasses()
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }

    //        try
    //        {
    //            var classDto = await _classService.GetAllClassesWithProfNameAndCourseNameAsync();
    //            return Ok(classDto);
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //    }

    //    [HttpGet("ClassById/{id:int}")]
    //    public async Task<IActionResult> ClassById(int id)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }

    //            var courseDto = await _classService.GetClassByIdAsync(id);
    //            return Ok(courseDto);
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //    }

    //    [HttpGet("CountOfClasses")]
    //    public async Task<IActionResult> CountOfClasses()
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }
    //        try
    //        {
    //            var count = await _classService.GetClassCountAsync();
    //            return Ok(new { countOfClasses = count });
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //    }

    //    [HttpGet("AllNamesLocations")]
    //    public async Task<IActionResult> GetAllName()
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }
    //        try
    //        {
    //            var names = await _classService.GetAllLocationsNameAsync();
    //            return Ok(names);
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //    }

    //    [HttpDelete("DeleteClass/{id}")]
    //    public async Task<IActionResult> DeleteClass(int id)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }
    //        try
    //        {
    //            await _classService.DeleteClassAsync(id);
    //            return NoContent(); // 204 No Content
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //    }
    //}
}

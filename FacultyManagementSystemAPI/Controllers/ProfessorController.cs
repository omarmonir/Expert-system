using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Services.Implementes;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessorController(IProfessorService professorService) : ControllerBase
    {
        private readonly IProfessorService _professorService = professorService;


        [HttpPost("CreateProfessor")]
        public async Task<IActionResult> CreateProfessor([FromBody] CreateProfessorDto createProfessorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _professorService.AddAsync(createProfessorDto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateProfessor/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProfessorDto updateProfessorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _professorService.UpdateAsync(id, updateProfessorDto);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllProfessors")]
        public async Task<IActionResult> GetAllProfessors()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var professorDto = await _professorService.GetAllAsync();
                return Ok(professorDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       [HttpGet("GetProfessorById/{id:int}")]
       public async Task<IActionResult> GetByIdWith(int id)
       {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var professorDto = await _professorService.GetByIdAsync(id);
                return Ok(professorDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
       } 
        
       [HttpGet("GetProfessorByDepartmentId/{id:int}")]
       public async Task<IActionResult> GetProfessorByDepartmentId(int id)
       {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var professorDto = await _professorService.GetByDepartmentIdAsync(id);
                return Ok(professorDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
       } 
        
       [HttpGet("GetCoursesByProfessorId/{id:int}")]
       public async Task<IActionResult> GetCoursesByProfessorId(int id)
       {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var professorDto = await _professorService.GetCoursesByProfessorIdAsync(id);
                return Ok(professorDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
       }


        [HttpDelete("DeleteProfessor/{id:int}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _professorService.DeleteAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

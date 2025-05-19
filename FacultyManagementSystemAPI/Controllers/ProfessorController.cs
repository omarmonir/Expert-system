using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessorController(IProfessorService professorService) : ControllerBase
    {
        private readonly IProfessorService _professorService = professorService;
        [HttpPost("AddProfessor")]
        //[Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Add([FromForm] CreateProfessorDto createProfessorDto)
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

        [HttpPost("AddMultipleProfessors")]
        //[Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> AddMultiple([FromBody] List<CreateProfessorDto> professors)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                List<string> results = new List<string>();
                foreach (var professor in professors)
                {
                    try
                    {
                        await _professorService.AddMultipleAsync(professor);
                        results.Add($"تم إضافة الدكتور {professor.FullName} بنجاح");
                    }
                    catch (Exception ex)
                    {
                        results.Add($"فشل إضافة الدكتور {professor.FullName}: {ex.Message}");
                    }
                }

                return Ok(new { Message = "تمت معالجة إضافة الأساتذة", Results = results });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RegisterProfessor")]
        public async Task<IActionResult> CreateProfessor([FromForm] CreateProfessorDto createProfessorDto)
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

        [HttpGet("AllNamesProfessors")]
        public async Task<IActionResult> GetAllName()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var names = await _professorService.GetAllProfessorsNameAsync();
                return Ok(names);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

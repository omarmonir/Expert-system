﻿using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetAllProfessors([FromQuery] int pageNumber = 1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var professorDto = await _professorService.GetAllAsync(pageNumber);
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
        
        [HttpGet("ProfessorProfile")]
        public async Task<IActionResult> GetById()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var professorIdClaim = User.FindFirst("ProfessorId");
                if (professorIdClaim == null)
                    return Unauthorized("Invalid token: ProfessorId not found");

                int professorId = int.Parse(professorIdClaim.Value);
                var professorDto = await _professorService.GetByIdAsync(professorId);
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

        [Authorize(Roles = ConstantRoles.Professor)]
        [HttpGet("GetCoursesByProfessorId")]
        public async Task<IActionResult> GetCoursesByProfessorId()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var professorIdClaim = User.FindFirst("ProfessorId");
                if (professorIdClaim == null)
                    return Unauthorized("Invalid token: ProfessorId not found");

                int professorId = int.Parse(professorIdClaim.Value);

                var professorDto = await _professorService.GetCoursesByProfessorIdAsync(professorId);
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

        [HttpGet("AllPositionsProfessors")]
        public async Task<IActionResult> GetAllProfessorsPosition()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var names = await _professorService.GetAllProfessorsPositionAsync();
                return Ok(names);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FilterByProfessorNameAndDepartmentName")]
        public async Task<ActionResult<IEnumerable<ProfessorDto>>> GetProfessorsByFilters(
        [FromQuery] string? departmentName,
        [FromQuery] string? professorName,
        [FromQuery] string? Position)
        {
            try
            {
                var professors = await _professorService.GetProfessorsByFiltersAsync(
                    departmentName,
                    professorName,
                    Position);

                return Ok(professors);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving professors.");
            }
        }
    }
}


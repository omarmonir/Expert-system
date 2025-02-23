using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StudentController(IStudentService studentService) : ControllerBase
	{
		private readonly IStudentService _studentService = studentService;

		[HttpGet("GetStudentsWithDepartmentName")]
		public async Task<IActionResult> GetAllWithDepartmentName()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var studentsDto = await _studentService.GetAllWithDepartmentNameAsync();
				return Ok(studentsDto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("GetStudentById{id:int}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var studentDto = await _studentService.GetByIdWithDepartmentNameAsync(id);
				return Ok(studentDto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("SearchByName{name}")]
		public async Task<IActionResult> GetByName(string name)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var studentsDto = await _studentService.GetByNameWithDepartmentNameAsync(name);
				return Ok(studentsDto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet("GetGradesOFStudentById{id:int}")]
		public async Task<IActionResult> GetGradesOFStudentById(int id)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var studentDto = await _studentService.GetByIdWithHisGradeAsync(id);
				return Ok(studentDto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("AddStudent")]
		public async Task<IActionResult> Add([FromForm] CreateStudentDto createStudentDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				await _studentService.AddAsync(createStudentDto);
				return StatusCode(201);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("UpdateStudent{id}")]
		public async Task<IActionResult> Update(int id, [FromForm] UpdateStudentDto updateStudentDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				await _studentService.UpdateAsync(id, updateStudentDto);
				return NoContent(); // 204 No Content
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("DeleteStudent{id:int}")]
		public async Task<IActionResult> DeleteById(int id)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				await _studentService.DeleteAsync(id);
				return NoContent(); // 204 No Content
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}

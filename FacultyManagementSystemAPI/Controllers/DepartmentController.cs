using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController(IDepartmentService departmentService) : ControllerBase
    {
        private readonly IDepartmentService _departmentService = departmentService;

        [HttpGet("AllDepartments")]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var departments = await _departmentService.GetAllAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AllNamesDepartment")]
        public async Task<IActionResult> GetAllName()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var names = await _departmentService.GetDepartmentNameAsync();
                return Ok(names);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Department/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var department = await _departmentService.GetByIdAsync(id);
                return Ok(department);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DepartmentId/{departmentName}")]
        public async Task<IActionResult> GetIdOfDepartmentByName(string departmentName)
        {
            Console.WriteLine($"Received departmentName: {departmentName}"); // طباعة الاسم المستلم

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var deptId = await _departmentService.GetIdOfDepartmentByNameAsync(departmentName);

                return Ok(new { departmentId = deptId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

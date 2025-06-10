using FacultyManagementSystemAPI.Services.Implementes;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController(IDivisionService divisionService) : ControllerBase
    {
        [HttpGet("AllNamesDivsion")]
        public async Task<IActionResult> GetAllName()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var names = await divisionService.GetAllDivisionNameAsync();
                return Ok(names);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

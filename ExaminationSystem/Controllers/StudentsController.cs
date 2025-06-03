using ExaminationSystem.Services.StudentsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController(IStudentService studentService) : ControllerBase
    {
        private readonly IStudentService _studentService = studentService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _studentService.GetAllAsync();

            return Ok(response);
        }
    }
}

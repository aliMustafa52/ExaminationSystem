using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Courses;
using ExaminationSystem.Contracts.Instructors;
using ExaminationSystem.Services.InstructorsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController(IInstructorService instructorService) : ControllerBase
    {
        private readonly IInstructorService _instructorService = instructorService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _instructorService.GetAllAsync();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _instructorService.GetByIdAsync(id);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("")]
        public async Task<IActionResult> AddCourse([FromBody] InstructorRequest request)
        {
            var response = await _instructorService.AddInstructorAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse([FromRoute] int id, [FromBody] InstructorRequest request)
        {
            var result = await _instructorService.UpdateInstructorAsync(id, request);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] int id, CancellationToken cancellationToken)
        {

            var result = await _instructorService.DeleteInstructorAsync(id, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }
    }
}

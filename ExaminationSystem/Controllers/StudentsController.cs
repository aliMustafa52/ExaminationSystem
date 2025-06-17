using ExaminationSystem.Abstractions;
using ExaminationSystem.Services.ExamsService;
using ExaminationSystem.Services.StudentsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExaminationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController(IStudentService studentService,
        IExamService examService) : ControllerBase
    {
        private readonly IStudentService _studentService = studentService;
        private readonly IExamService _examService = examService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _studentService.GetAllAsync();

            return Ok(response);
        }

        [HttpPost("{studentId}/assign-to-exam/{examId}")]
        [Authorize]
        public async Task<IActionResult> AssignExamToStudent([FromRoute] int examId, [FromRoute] int studentId, CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _examService.AssignExamToStudent(examId, studentId, instructorId, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        [HttpDelete("{studentId}/remove-from-exam/{examId}")]
        [Authorize]
        public async Task<IActionResult> RemoveExamToStudent([FromRoute] int examId, [FromRoute] int studentId, CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _examService.RemoveExamFromStudent(examId, studentId, instructorId, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }
    }
}

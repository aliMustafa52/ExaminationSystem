using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Exams;
using ExaminationSystem.Entities;
using ExaminationSystem.Services.ExamsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExaminationSystem.Controllers
{
    [Route("api/courses/{courseId}/[controller]")]
    [ApiController]
    
    public class ExamsController(IExamService examService) : ControllerBase
    {
        private readonly IExamService _examService = examService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromRoute] int courseId, CancellationToken cancellationToken)
        {
            var result = await _examService.GetAllAsync(courseId, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("for-instructor")]
        [Authorize]
        public async Task<IActionResult> GetAllForTeacher([FromRoute] int courseId, CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _examService.GetAllForTeacherAsync(courseId, instructorId, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("~/api/[controller]/for-student")]
        [Authorize]
        public async Task<IActionResult> GetAllForStudent(CancellationToken cancellationToken)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _examService.GetAllForStudentAsync(studentId, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int courseId, [FromRoute] int id)
        {
            var result = await _examService.GetByIdAsync(courseId, id);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("{id}/for-instructor")]
        [Authorize]
        public async Task<IActionResult> GetByIdForTeacher([FromRoute] int courseId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _examService.GetByIdForTeacherAsync(courseId, id, instructorId, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("~/api/[controller]/{id}/for-student")]
        [Authorize]
        public async Task<IActionResult> GetByIdForStudent([FromRoute] int id, CancellationToken cancellationToken)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _examService.GetByIdForStudentAsync(id, studentId, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("~/api/[controller]/{id}/assign-random-questions")]
        [Authorize]
        public async Task<IActionResult> AssignRandomQuestions([FromRoute] int id, [FromBody] AutoExamQuestionsRequest request, CancellationToken cancellationToken)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _examService.AssignRandomQuestionsToExam(id, studentId, request, cancellationToken);
            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        [HttpPost("")]
        [Authorize]
        public async Task<ActionResult<ExamResponse>> Add([FromRoute] int courseId, [FromBody] AddExamRequest request, CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _examService.AddExamAsync(courseId, instructorId , request, cancellationToken);

            return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { courseId, id = result.Value.Id }, result.Value)
            : result.ToProblem();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int courseId, [FromRoute] int id, [FromBody] UpdateExamRequest request, CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _examService.UpdateExamAsync(courseId, id, instructorId, request, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int courseId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _examService.DeleteExamAsync(courseId, id, instructorId, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        
    }
}

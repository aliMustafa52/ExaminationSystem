using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Exams;
using ExaminationSystem.Contracts.Questions;
using ExaminationSystem.Services.QuestionsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExaminationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _questionService.GetAllQuestionsAsync(instructorId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _questionService.GetQuestionByIdAsync(instructorId, id);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("")]
        public async Task<ActionResult<ExamResponse>> Add([FromRoute] int courseId, [FromBody] AddQuestionRequest request, CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _questionService.AddQuestionAsync(instructorId, request, cancellationToken);

            return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { courseId, id = result.Value.Id }, result.Value)
            : result.ToProblem();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateQuestionRequest request, CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _questionService.UpdateQuestionAsync( instructorId, id, request, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _questionService.DeleteQuestionAsync(instructorId, id, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }
    }
}

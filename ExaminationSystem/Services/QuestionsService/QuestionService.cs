using Azure.Core;
using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Exams;
using ExaminationSystem.Contracts.Questions;
using ExaminationSystem.Entities;
using ExaminationSystem.Entities.Enums;
using ExaminationSystem.Errors;
using ExaminationSystem.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services.QuestionsService
{
    public class QuestionService(UserManager<AppUser> userManager, 
        IGeneralRepository<Question> questionRepository,
        IGeneralRepository<Choice> choiceRepository) : IQuestionService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IGeneralRepository<Question> _questionRepository = questionRepository;
        private readonly IGeneralRepository<Choice> _choiceRepository = choiceRepository;

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllQuestionsAsync(string instructorId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Include(x => x.Instructor)
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<IEnumerable<QuestionResponse>>(UserErrors.UserNotFound);

            if (instructorUser.Instructor is null || !instructorUser.Instructor.IsActive)
                return Result.Failure<IEnumerable<QuestionResponse>>(InstructorErrors.InstructorNotFound);

            var questions = await _questionRepository.Get(x => x.InstructorId == instructorUser.Instructor.Id && x.IsActive)
                    .Include(x => x.Choices.Where(c => c.IsActive))
                    .ToListAsync(cancellationToken);

            var response = questions.Adapt<IEnumerable<QuestionResponse>>();

            return Result.Success(response);
        }

        public async Task<Result<QuestionResponse>> GetQuestionByIdAsync(string instructorId, int questionId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Include(x => x.Instructor)
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<QuestionResponse>(UserErrors.UserNotFound);

            if (instructorUser.Instructor is null || !instructorUser.Instructor.IsActive)
                return Result.Failure<QuestionResponse>(InstructorErrors.InstructorNotFound);

            var question = await _questionRepository.Get(x => x.Id== questionId && x.InstructorId == instructorUser.Instructor.Id && x.IsActive)
                    .Include(x => x.Choices.Where(c => c.IsActive))
                    .SingleOrDefaultAsync(cancellationToken);
            if(question is null)
            {
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
            }

            var response = question.Adapt<QuestionResponse>();

            return Result.Success(response);
        }

        public async Task<Result<QuestionResponse>> AddQuestionAsync(string instructorId,AddQuestionRequest request, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Include(x => x.Instructor)
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<QuestionResponse>(UserErrors.UserNotFound);

            if (instructorUser.Instructor is null || !instructorUser.Instructor.IsActive)
                return Result.Failure<QuestionResponse>(InstructorErrors.InstructorNotFound);

            var question = request.Adapt<Question>();
            question.InstructorId = instructorUser.Instructor.Id;

            var addedQuestion =  await _questionRepository.AddAsync(question);

            var response = addedQuestion.Adapt<QuestionResponse>();

            return Result.Success(response);
        }

        public async Task<Result> UpdateQuestionAsync(string instructorId, int questionId, UpdateQuestionRequest request, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Include(x => x.Instructor)
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<QuestionResponse>(UserErrors.UserNotFound);

            if (instructorUser.Instructor is null || !instructorUser.Instructor.IsActive)
                return Result.Failure<QuestionResponse>(InstructorErrors.InstructorNotFound);

            var question = await _questionRepository.Get(x => x.Id == questionId && x.InstructorId == instructorUser.Instructor.Id && x.IsActive)
                    .SingleOrDefaultAsync(cancellationToken);
            if (question is null)
            {
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
            }

            await _questionRepository.UpdateAsync(x => x.Id == questionId && x.InstructorId == instructorUser.Instructor.Id && x.IsActive,
                s => s
                    .SetProperty(q => q.Text, request.Text)
                    .SetProperty(q => q.DifficultyLevel, request.DifficultyLevel)
                    );

            var currentChoices = await _choiceRepository.Get(c => c.QuestionId == questionId && c.IsActive).ToListAsync(cancellationToken);

            //Remove Choices
            var choicesToRemove = currentChoices.Where(c => !request.Choices.Any(x => x.Id == c.Id)).ToList();
            if (choicesToRemove.Count != 0)
            {
                var idsToRemove = choicesToRemove.Select(x => x.Id).ToList();
                await _choiceRepository.UpdateAsync(c => idsToRemove.Contains(c.Id),
                        s => s.SetProperty(x => x.IsActive, false));
            }

            // Add Or Update Choices
            foreach (var choice in request.Choices)
            {
                var currentChoice = currentChoices.SingleOrDefault(c => c.Id == choice.Id);
                if (currentChoice is null)
                {
                    await _choiceRepository.AddAsync(new Choice
                    {
                        Content = choice.Content,
                        IsCorrect = choice.IsCorrect,
                        QuestionId = questionId
                    });
                }
                else
                {
                    await _choiceRepository.UpdateAsync(c => c.Id == currentChoice.Id,
                        s => s
                        .SetProperty(x => x.Content, choice.Content)
                        .SetProperty(x => x.IsCorrect, choice.IsCorrect)
                        );
                }
            }

            return Result.Success();
        }

        public async Task<Result> DeleteQuestionAsync(string instructorId, int questionId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Include(x => x.Instructor)
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<QuestionResponse>(UserErrors.UserNotFound);

            if (instructorUser.Instructor is null || !instructorUser.Instructor.IsActive)
                return Result.Failure<QuestionResponse>(InstructorErrors.InstructorNotFound);

            var question = await _questionRepository.Get(x => x.Id == questionId && x.InstructorId == instructorUser.Instructor.Id && x.IsActive)
                    .SingleOrDefaultAsync(cancellationToken);
            if (question is null)
            {
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
            }

            await _questionRepository.UpdateAsync(x => x.Id == questionId && x.InstructorId == instructorUser.Instructor.Id && x.IsActive,
            s => s
                   .SetProperty(q => q.IsActive, false)
                   );

            await _choiceRepository.UpdateAsync(x => x.QuestionId == questionId && x.IsActive,
            s => s
                   .SetProperty(q => q.IsActive, false)
                   );

            return Result.Success();
        }
    }
}

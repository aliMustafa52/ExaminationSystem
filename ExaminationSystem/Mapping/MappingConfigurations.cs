using ExaminationSystem.Contracts.Authentication;
using ExaminationSystem.Contracts.Choices;
using ExaminationSystem.Contracts.Questions;
using ExaminationSystem.Entities;
using Mapster;

namespace ExaminationSystem.Mapping
{
    public class MappingConfigurations : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RegisterRequest, AppUser>()
                .Map(dest => dest.UserName, src => src.Email);

            config.NewConfig<InstructorRegisterRequest, AppUser>()
                .Map(dest => dest.UserName, src => src.Email);

            config.NewConfig<StudentRegisterRequest, AppUser>()
                .Map(dest => dest.UserName, src => src.Email); 
            
            config.NewConfig<AddQuestionRequest, Question>()
                .Map(dest => dest.Choices, src => src.Choices.Select(x => new Choice { Content = x.Content, IsCorrect = x.IsCorrect }));

            config.NewConfig<Question, QuestionResponse>()
                .Map(dest => dest.Choices, src => src.Choices.Select(x => new ChoiceResponse(x.Id, x.Content,x.IsCorrect)));
        }
    }
}

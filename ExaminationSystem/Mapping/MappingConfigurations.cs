using ExaminationSystem.Contracts.Authentication;
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
        }
    }
}

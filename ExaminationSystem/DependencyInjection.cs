using ExaminationSystem.Data;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;

namespace ExaminationSystem
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services
                .AddDbContextConfig(configuration)
                .AddMapsterConfig()
                .AddFluentValidationConfig();

            return services;
        }

        private static IServiceCollection AddDbContextConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("connectionString name DefaultConnection is not found");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return services;
        }

        private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
        {
            var mappingConfig = TypeAdapterConfig.GlobalSettings;
            mappingConfig.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(new Mapper(mappingConfig));

            return services;
        }

        private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
        {
            services
                .AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}

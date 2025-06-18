using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Instructors;
using ExaminationSystem.Contracts.Students;
using ExaminationSystem.Entities;
using ExaminationSystem.Entities.Enums;
using ExaminationSystem.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services.StudentsService
{
    public class StudentService(IGeneralRepository<Student> StudentRepository,
        UserManager<AppUser> userManager) : IStudentService
    {
        private readonly IGeneralRepository<Student> _studentRepository = StudentRepository;
        private readonly UserManager<AppUser> _userManager = userManager;

        public async Task<IEnumerable<StudentResponse>> GetAllAsync()
        {
            var studetns = await _userManager.Users
                .Where(u => !u.IsDisabled && u.UserType == UserType.Student)
                .Select(u => new StudentResponse(u.Student!.Id, u.FirstName, u.LastName, u.Student.Address))
                .ToListAsync();

            //var students = await _studentRepository.GetAll().ToListAsync();

            return studetns;
        }
    }
}

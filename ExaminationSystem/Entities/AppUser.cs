using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsDisabled { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; } = [];
    }
}

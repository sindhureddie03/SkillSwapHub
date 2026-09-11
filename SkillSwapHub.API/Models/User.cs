namespace SkillSwapHub.API.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? Bio { get; set; }

        public string? ExperienceLevel { get; set; }

        public string? Availability { get; set; }

        public int? PreferredSessionDuration { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
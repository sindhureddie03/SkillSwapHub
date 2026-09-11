namespace SkillSwapHub.API.DTOs
{
    public class UpdateProfileRequest
    {
        public string? Bio { get; set; }

        public string? ExperienceLevel { get; set; }

        public string? Availability { get; set; }

        public int? PreferredSessionDuration { get; set; }
    }
}
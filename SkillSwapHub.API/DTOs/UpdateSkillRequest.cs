namespace SkillSwapHub.API.DTOs
{
    public class UpdateSkillRequest
    {
        public string SkillName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
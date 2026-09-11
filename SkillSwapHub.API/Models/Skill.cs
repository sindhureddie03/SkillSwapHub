namespace SkillSwapHub.API.Models
{
    public class Skill
    {
        public int SkillId { get; set; }

        public string SkillName { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
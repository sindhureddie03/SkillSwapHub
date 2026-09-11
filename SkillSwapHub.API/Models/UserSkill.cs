namespace SkillSwapHub.API.Models
{
    public class UserSkill
    {
        public int UserSkillId { get; set; }

        public int UserId { get; set; }

        public int SkillId { get; set; }

        public string SkillType { get; set; } = string.Empty;

        public string? SkillLevel { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
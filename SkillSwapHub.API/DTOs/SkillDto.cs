namespace SkillSwapHub.API.DTOs
{
    public class SkillDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
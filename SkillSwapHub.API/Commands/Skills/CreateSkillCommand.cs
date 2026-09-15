namespace SkillSwapHub.API.Commands.Skills
{
    public class CreateSkillCommand
    {
        public string SkillName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Description { get; set; }
    }
}
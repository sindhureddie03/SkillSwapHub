using SkillSwapHub.API.Models;

namespace SkillSwapHub.API.Commands.Skills
{
    public class UpdateSkillCommand
    {
        public Skill Skill { get; set; } = new();
    }
}
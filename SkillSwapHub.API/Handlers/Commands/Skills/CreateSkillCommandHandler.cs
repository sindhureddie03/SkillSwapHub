using SkillSwapHub.API.Commands.Skills;
using SkillSwapHub.API.Models;
using SkillSwapHub.API.Repositories;

namespace SkillSwapHub.API.Handlers.Commands.Skills
{
    public class CreateSkillCommandHandler
    {
        private readonly SkillRepository _skillRepository;

        public CreateSkillCommandHandler(SkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<int> Handle(CreateSkillCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.SkillName))
            {
                throw new ArgumentException("Skill name is required.");
            }

            var skill = new Skill
            {
                SkillName = command.SkillName,
                Category = command.Category,
                Description = command.Description
            };

            return await _skillRepository.CreateSkillAsync(skill);
        }
    }
}
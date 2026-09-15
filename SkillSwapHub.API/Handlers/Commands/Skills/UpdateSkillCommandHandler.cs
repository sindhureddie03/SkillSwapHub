using SkillSwapHub.API.Commands.Skills;
using SkillSwapHub.API.Repositories;

namespace SkillSwapHub.API.Handlers.Commands.Skills
{
    public class UpdateSkillCommandHandler
    {
        private readonly SkillRepository _skillRepository;

        public UpdateSkillCommandHandler(SkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<bool> Handle(UpdateSkillCommand command)
        {
            return await _skillRepository.UpdateSkillAsync(command.Skill);
        }
    }
}
using SkillSwapHub.API.Commands.Skills;
using SkillSwapHub.API.Repositories;

namespace SkillSwapHub.API.Handlers.Commands.Skills
{
    public class DeleteSkillCommandHandler
    {
        private readonly SkillRepository _skillRepository;

        public DeleteSkillCommandHandler(SkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<bool> Handle(DeleteSkillCommand command)
        {
            return await _skillRepository.DeleteSkillAsync(command.SkillId);
        }
    }
}
using SkillSwapHub.API.Queries.Skills;
using SkillSwapHub.API.Repositories;

namespace SkillSwapHub.API.Handlers.Queries.Skills
{
    public class GetAllSkillsQueryHandler
    {
        private readonly SkillRepository _skillRepository;

        public GetAllSkillsQueryHandler(SkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<object> Handle(GetAllSkillsQuery query)
        {
            return await _skillRepository.GetAllSkillsAsync();
        }
    }
}
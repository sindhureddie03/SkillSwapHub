using SkillSwapHub.API.Repositories;
using SkillSwapHub.API.Queries.Skills;

namespace SkillSwapHub.API.Handlers.Queries.Skills
{
    public class GetSkillByIdQueryHandler
    {
        private readonly SkillRepository _skillRepository;

        public GetSkillByIdQueryHandler(SkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<object?> Handle(GetSkillByIdQuery query)
        {
            return await _skillRepository.GetSkillByIdAsync(query.SkillId);
        }
    }
}
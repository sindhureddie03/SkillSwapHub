using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwapHub.API.Repositories;

namespace SkillSwapHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SkillsController : ControllerBase
    {
        private readonly SkillRepository _skillRepository;

        public SkillsController(SkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _skillRepository.GetAllSkillsAsync();

            return Ok(skills);
        }
    }
}
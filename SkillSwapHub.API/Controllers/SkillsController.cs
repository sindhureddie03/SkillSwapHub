using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwapHub.API.Models;
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

        [HttpPost]
        public async Task<IActionResult> CreateSkill([FromBody] Skill skill)
        {
            if (string.IsNullOrWhiteSpace(skill.SkillName))
            {
                return BadRequest("Skill name is required.");
            }

            var skillId = await _skillRepository.CreateSkillAsync(skill);

            return Ok(new
            {
                SkillId = skillId,
                Message = "Skill created successfully."
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            var skill = await _skillRepository.GetSkillByIdAsync(id);

            if (skill == null)
                return NotFound("Skill not found.");

            return Ok(skill);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] Skill skill)
        {
            skill.SkillId = id;

            var updated = await _skillRepository.UpdateSkillAsync(skill);

            if (!updated)
                return NotFound("Skill not found.");

            return Ok("Skill updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var deleted = await _skillRepository.DeleteSkillAsync(id);

            if (!deleted)
                return NotFound("Skill not found.");

            return Ok("Skill deleted successfully.");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwapHub.API.Commands.Skills;
using SkillSwapHub.API.DTOs;
using SkillSwapHub.API.Handlers.Commands.Skills;
using SkillSwapHub.API.Handlers.Queries.Skills;
using SkillSwapHub.API.Models;
using SkillSwapHub.API.Queries.Skills;
using SkillSwapHub.API.Repositories;

namespace SkillSwapHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SkillsController : ControllerBase
    {
        private readonly SkillRepository _skillRepository;
        private readonly CreateSkillCommandHandler _createSkillHandler;
        private readonly GetAllSkillsQueryHandler _getAllSkillsHandler;
        private readonly GetSkillByIdQueryHandler _getSkillByIdHandler;
        private readonly UpdateSkillCommandHandler _updateSkillHandler;
        private readonly DeleteSkillCommandHandler _deleteSkillHandler;

        public SkillsController(
    SkillRepository skillRepository,
    CreateSkillCommandHandler createSkillHandler,
    GetAllSkillsQueryHandler getAllSkillsHandler,
    GetSkillByIdQueryHandler getSkillByIdHandler,
    UpdateSkillCommandHandler updateSkillHandler,
    DeleteSkillCommandHandler deleteSkillHandler)
        {
            _skillRepository = skillRepository;
            _createSkillHandler = createSkillHandler;
            _getAllSkillsHandler = getAllSkillsHandler;
            _getSkillByIdHandler = getSkillByIdHandler;
            _updateSkillHandler = updateSkillHandler;
            _deleteSkillHandler = deleteSkillHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var query = new GetAllSkillsQuery();

            var skills = await _getAllSkillsHandler.Handle(query);

            return Ok(skills);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSkill(
            [FromBody] CreateSkillCommand command)
        {
            try
            {
                var skillId = await _createSkillHandler.Handle(command);

                return Ok(new
                {
                    SkillId = skillId,
                    Message = "Skill created successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            var query = new GetSkillByIdQuery
            {
                SkillId = id
            };

            var skill = await _getSkillByIdHandler.Handle(query);

            if (skill == null)
                return NotFound();

            return Ok(skill);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkill(
    int id,
    [FromBody] Skill skill)
        {
            skill.SkillId = id;

            var command = new UpdateSkillCommand
            {
                Skill = skill
            };

            var result = await _updateSkillHandler.Handle(command);

            if (!result)
                return NotFound();

            return Ok(new
            {
                Message = "Skill updated successfully."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var command = new DeleteSkillCommand
            {
                SkillId = id
            };

            var deleted = await _deleteSkillHandler.Handle(command);

            if (!deleted)
                return NotFound("Skill not found.");

            return Ok("Skill deleted successfully.");
        }
    }
}


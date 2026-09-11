using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwapHub.API.Models;
using SkillSwapHub.API.Repositories;
using System.Security.Claims;

namespace SkillSwapHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserSkillsController : ControllerBase
    {
        private readonly UserSkillRepository _userSkillRepository;

        public UserSkillsController(
            UserSkillRepository userSkillRepository)
        {
            _userSkillRepository = userSkillRepository;
        }


        // ============================================================
        // ADD SKILL
        // ============================================================

        [HttpPost]
        public async Task<IActionResult> AddUserSkill(
            UserSkill userSkill)
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            userSkill.UserId = userId;

            await _userSkillRepository.AddUserSkillAsync(
                userSkill);

            return Ok(new
            {
                message = "Skill added successfully."
            });
        }


        // ============================================================
        // GET MY SKILLS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> GetMySkills()
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var skills = await _userSkillRepository
                .GetMySkillsAsync(userId);

            return Ok(skills);
        }


        // ============================================================
        // UPDATE SKILL
        // ============================================================

        [HttpPut("{userSkillId}")]
        public async Task<IActionResult> UpdateUserSkill(
            int userSkillId,
            UserSkill userSkill)
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            bool updated =
                await _userSkillRepository.UpdateUserSkillAsync(
                    userSkillId,
                    userId,
                    userSkill.SkillType,
                    userSkill.SkillLevel);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Skill not found."
                });
            }

            return Ok(new
            {
                message = "Skill updated successfully."
            });
        }


        // ============================================================
        // DELETE SKILL
        // ============================================================

        [HttpDelete("{userSkillId}")]
        public async Task<IActionResult> DeleteUserSkill(
            int userSkillId)
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            bool deleted =
                await _userSkillRepository.DeleteUserSkillAsync(
                    userSkillId,
                    userId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Skill not found."
                });
            }

            return Ok(new
            {
                message = "Skill deleted successfully."
            });
        }
    }
}
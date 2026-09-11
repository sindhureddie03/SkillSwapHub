using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwapHub.API.Repositories;
using System.Security.Claims;
using SkillSwapHub.API.DTOs;

namespace SkillSwapHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public ProfileController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userIdValue = User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

            if (string.IsNullOrEmpty(userIdValue))
            {
                return Unauthorized(new
                {
                    message = "User ID not found in token"
                });
            }

            int userId = int.Parse(userIdValue);

            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            return Ok(new
            {
                userId = user.UserId,
                name = user.Name,
                email = user.Email,
                bio = user.Bio,
                experienceLevel = user.ExperienceLevel,
                availability = user.Availability,
                preferredSessionDuration = user.PreferredSessionDuration,
                createdAt = user.CreatedAt
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(
    UpdateProfileRequest request)
        {
            var userIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            await _userRepository.UpdateProfileAsync(
                userId,
                request.Bio,
                request.ExperienceLevel,
                request.Availability,
                request.PreferredSessionDuration);

            return Ok(new
            {
                message = "Profile updated successfully."
            });
        }
    }
}
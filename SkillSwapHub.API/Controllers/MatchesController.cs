using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwapHub.API.Repositories;
using System.Security.Claims;

namespace SkillSwapHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MatchesController : ControllerBase
    {
        private readonly MatchRepository _matchRepository;

        public MatchesController(
            MatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }


        // ============================================================
        // GET SKILL MATCHES
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> GetMatches()
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var matches =
                await _matchRepository.GetMatchesAsync(userId);

            return Ok(matches);
        }
    }
}
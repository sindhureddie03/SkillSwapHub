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
    public class SessionRequestsController : ControllerBase
    {
        private readonly SessionRequestRepository _sessionRequestRepository;

        public SessionRequestsController(
            SessionRequestRepository sessionRequestRepository)
        {
            _sessionRequestRepository = sessionRequestRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSessionRequest(
            SessionRequest request)
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            request.RequesterUserId = userId;

            if (request.ReceiverUserId == userId)
            {
                return BadRequest(new
                {
                    message = "You cannot send a session request to yourself."
                });
            }

            await _sessionRequestRepository
                .AddSessionRequestAsync(request);

            return Ok(new
            {
                message = "Session request sent successfully."
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetReceivedRequests()
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var requests =
                await _sessionRequestRepository
                    .GetReceivedRequestsAsync(userId);

            return Ok(requests);
        }

        [HttpPut("{sessionRequestId}")]
        public async Task<IActionResult> UpdateRequestStatus(
    int sessionRequestId,
    [FromBody] string status)
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            if (status != "Accepted" && status != "Rejected")
            {
                return BadRequest(new
                {
                    message = "Status must be Accepted or Rejected."
                });
            }

            bool updated =
                await _sessionRequestRepository
                    .UpdateRequestStatusAsync(
                        sessionRequestId,
                        userId,
                        status);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Session request not found."
                });
            }

            return Ok(new
            {
                message = "Session request " +
                          status.ToLower() +
                          " successfully."
            });
        }
    }
}
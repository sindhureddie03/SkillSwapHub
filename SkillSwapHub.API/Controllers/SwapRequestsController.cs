using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwapHub.API.Models;
using SkillSwapHub.API.Repositories;

namespace SkillSwapHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SwapRequestsController : ControllerBase
    {
        private readonly SwapRequestRepository _swapRequestRepository;

        public SwapRequestsController(
            SwapRequestRepository swapRequestRepository)
        {
            _swapRequestRepository = swapRequestRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSwapRequest(
            [FromBody] SwapRequest request)
        {
            var requestId =
                await _swapRequestRepository.CreateSwapRequestAsync(request);

            return Ok(new
            {
                SwapRequestId = requestId,
                Message = "Swap request created successfully."
            });
        }

        [HttpGet("received/{receiverId}")]
        public async Task<IActionResult> GetReceivedRequests(int receiverId)
        {
            var requests =
                await _swapRequestRepository.GetReceivedRequestsAsync(receiverId);

            return Ok(requests);
        }

        [HttpPut("{id}/accept")]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            var updated =
                await _swapRequestRepository.UpdateStatusAsync(id, "ACCEPTED");

            if (!updated)
                return NotFound("Swap request not found.");

            return Ok("Swap request accepted successfully.");
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var updated =
                await _swapRequestRepository.UpdateStatusAsync(id, "REJECTED");

            if (!updated)
                return NotFound("Swap request not found.");

            return Ok("Swap request rejected successfully.");
        }
    }
}
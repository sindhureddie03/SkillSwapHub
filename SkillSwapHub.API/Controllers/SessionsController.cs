
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SkillSwapHub.API.Data;
using SkillSwapHub.API.Models;
using SkillSwapHub.API.Repositories;
using System.Security.Claims;

namespace SkillSwapHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly SessionRepository _sessionRepository;
        private readonly DbConnectionFactory _dbConnectionFactory;

        public SessionsController(
            SessionRepository sessionRepository,
            DbConnectionFactory dbConnectionFactory)
        {
            _sessionRepository = sessionRepository;
            _dbConnectionFactory = dbConnectionFactory;
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> ScheduleSession(
            [FromBody] Session session)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            string query = @"
                SELECT Status
                FROM SessionRequests
                WHERE SessionRequestId = @SessionRequestId";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@SessionRequestId",
                session.SessionRequestId);

            await connection.OpenAsync();

            var status =
                await command.ExecuteScalarAsync();

            if (status == null)
            {
                return NotFound(
                    "Session request not found.");
            }

            if (status.ToString() != "Accepted")
            {
                return BadRequest(
                    "Session can only be scheduled for an accepted session request.");
            }

            if (session.DurationMinutes <= 0)
            {
                return BadRequest(
                    "Duration must be greater than 0.");
            }

            var sessionId =
                await _sessionRepository
                    .ScheduleSessionAsync(session);

            return Ok(new
            {
                SessionId = sessionId,
                Message = "Session scheduled successfully."
            });
        }


        [HttpGet("upcoming/{userId}")]
        public async Task<IActionResult> GetUpcomingSessions(
            int userId)
        {
            var sessions =
                await _sessionRepository
                    .GetUpcomingSessionsAsync(userId);

            return Ok(sessions);
        }

        [HttpGet("completed/{userId}")]
        public async Task<IActionResult> GetCompletedSessions(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int loggedInUserId = int.Parse(userIdClaim.Value);

            // Make sure the logged-in user can only access their own sessions
            if (loggedInUserId != userId)
                return Unauthorized();

            var sessions =
                await _sessionRepository.GetCompletedSessionsAsync(userId);

            return Ok(sessions);
        }

        [HttpPost("{sessionId}/join")]
        public async Task<IActionResult> JoinSession(
            int sessionId)
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId =
                int.Parse(userIdClaim.Value);

            bool joined =
                await _sessionRepository
                    .JoinSessionAsync(
                        sessionId,
                        userId);

            if (!joined)
            {
                return NotFound(new
                {
                    message =
                        "Session not found."
                });
            }

            return Ok(new
            {
                message =
                    "Joined session successfully."
            });
        }


        [HttpPost("{sessionId}/complete")]
        public async Task<IActionResult> CompleteSession(
            int sessionId)
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId =
                int.Parse(userIdClaim.Value);

            bool completed =
                await _sessionRepository
                    .CompleteSessionAsync(
                        sessionId,
                        userId);

            if (!completed)
            {
                return NotFound(new
                {
                    message =
                        "Session not found."
                });
            }

            return Ok(new
            {
                message =
                    "Session completion recorded successfully."
            });
        }
    }
}


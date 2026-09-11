using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwapHub.API.Data;

namespace SkillSwapHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseTestController : ControllerBase
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public DatabaseTestController(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        [Authorize]
        [HttpGet]
        public IActionResult TestConnection()
        {
            try
            {
                using var connection = _dbConnectionFactory.CreateConnection();

                connection.Open();

                return Ok("Database connection successful!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database connection failed: {ex.Message}");
            }
        }
    }
}
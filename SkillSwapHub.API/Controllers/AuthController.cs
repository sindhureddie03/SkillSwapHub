using Microsoft.AspNetCore.Mvc;
using SkillSwapHub.API.DTOs;
using SkillSwapHub.API.Services;

namespace SkillSwapHub.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            try
            {
                int userId = await _authService.RegisterAsync(request);

                return Ok(new
                {
                    message = "Registration successful",
                    userId = userId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Registration failed",
                    error = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(
                request.Email,
                request.Password
            );

            if (result.User == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
            }

            return Ok(new
            {
                message = "Login successful",
                token = result.Token,
                userId = result.User.UserId,
                name = result.User.Name,
                email = result.User.Email
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var token = await _authService.ForgotPasswordAsync(request.Email);

            if (token == null)
            {
                return NotFound(new
                {
                    message = "No account found with this email"
                });
            }

            return Ok(new
            {
                message = "Password reset token generated successfully",
                token = token
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(
                request.Token,
                request.NewPassword);

            if (!result)
            {
                return BadRequest("Invalid or expired reset token.");
            }

            return Ok("Password reset successfully.");
        }
    }

}
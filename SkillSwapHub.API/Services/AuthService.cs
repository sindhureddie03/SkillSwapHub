using Microsoft.IdentityModel.Tokens;
using SkillSwapHub.API.DTOs;
using SkillSwapHub.API.Models;
using SkillSwapHub.API.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkillSwapHub.API.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserRepository userRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<int> RegisterAsync(RegisterRequestDto request)
        {
            bool emailExists = await _userRepository.EmailExistsAsync(request.Email);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            int userId = await _userRepository.CreateUserAsync(user);

            return userId;
        }

        public async Task<(User? User, string? Token)> LoginAsync(
            string email,
            string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return (null, null);
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(
                password,
                user.PasswordHash
            );

            if (!passwordValid)
            {
                return (null, null);
            }

            // Create JWT token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(
                        _configuration["Jwt:ExpiryMinutes"]
                    )
                ),
                signingCredentials: credentials
            );

            string tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return (user, tokenString);
        }
    }
}
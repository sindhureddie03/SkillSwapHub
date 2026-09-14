using System.ComponentModel.DataAnnotations;

namespace SkillSwapHub.API.DTOs
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}
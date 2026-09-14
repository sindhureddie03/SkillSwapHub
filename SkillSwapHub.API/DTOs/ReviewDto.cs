namespace SkillSwapHub.API.DTOs
{
    public class ReviewDto
    {
        public int SessionId { get; set; }

        public int ReviewedUserId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
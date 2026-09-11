namespace SkillSwapHub.API.Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        public int SessionId { get; set; }

        public int ReviewerId { get; set; }

        public int ReviewedUserId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
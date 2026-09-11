namespace SkillSwapHub.API.Models
{
    public class SwapRequest
    {
        public int SwapRequestId { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public int OfferedSkillId { get; set; }

        public int RequestedSkillId { get; set; }

        public string? Message { get; set; }

        public DateTime? PreferredDate { get; set; }

        public TimeSpan? PreferredTime { get; set; }

        public int? DurationMinutes { get; set; }

        public string Status { get; set; } = "PENDING";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
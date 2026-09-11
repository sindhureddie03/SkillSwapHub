namespace SkillSwapHub.API.Models
{
    public class SessionRequest
    {
        public int SessionRequestId { get; set; }

        public int RequesterUserId { get; set; }

        public int ReceiverUserId { get; set; }

        public int SkillId { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime RequestedAt { get; set; }
    }
}
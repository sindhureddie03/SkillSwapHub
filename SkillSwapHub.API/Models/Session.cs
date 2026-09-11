namespace SkillSwapHub.API.Models
{
    public class Session
    {
        public int SessionId { get; set; }

        public int SwapRequestId { get; set; }

        public DateTime ScheduledDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public int DurationMinutes { get; set; }

        public string? MeetingLink { get; set; }

        public DateTime? TeacherJoinedAt { get; set; }

        public DateTime? LearnerJoinedAt { get; set; }

        public bool TeacherCompleted { get; set; }

        public bool LearnerCompleted { get; set; }

        public string Status { get; set; } = "SCHEDULED";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
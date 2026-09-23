namespace PollAndVotingSystem.Models;

    public enum ApplicationStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class CandidateApplication
    {
        public int Id { get; set; }

        public int ElectionId { get; set; }
        public Election? Election { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public string Statement { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public int? ReviewedByUserId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectionReason { get; set; }
    }

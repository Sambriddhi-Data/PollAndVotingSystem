namespace PollAndVotingSystem.DTOs.CandidateApplication;

public class CandidateApplicationResponseDto
{
    public int Id { get; set; }
    public int ElectionId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }
}
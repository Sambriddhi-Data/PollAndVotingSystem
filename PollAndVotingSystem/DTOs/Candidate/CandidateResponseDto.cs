namespace PollAndVotingSystem.DTOs.Candidate;

public class CandidateResponseDto
{
    public int Id { get; set; }
    public int ElectionId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
}
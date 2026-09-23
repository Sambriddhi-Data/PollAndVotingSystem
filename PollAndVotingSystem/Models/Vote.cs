namespace PollAndVotingSystem.Models;

public class Vote
{
    public int Id { get; set; }

    public int ElectionId { get; set; }
    public Election? Election { get; set; }

    public int CandidateId { get; set; }
    public Candidate? Candidate { get; set; }

    public int VoterId { get; set; }
    public User? Voter { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
namespace PollAndVotingSystem.Models;

public class Candidate
{
    public int Id { get; set; }

    public int ElectionId { get; set; }
    public Election? Election { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int ApplicationId { get; set; }
    public CandidateApplication? Application { get; set; }

    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
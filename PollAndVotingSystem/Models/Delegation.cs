namespace PollAndVotingSystem.Models;

public class Delegation
{
    public int Id { get; set; }

    public int ElectionId { get; set; }
    public Election? Election { get; set; }

    public int DelegatorId { get; set; }
    public User? Delegator { get; set; }

    public int DelegateId { get; set; }
    public User? Delegate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
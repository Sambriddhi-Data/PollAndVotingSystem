namespace PollAndVotingSystem.Models;


public enum ElectionStatus
{
    Draft,
    Active,
    Closed
}
public class Election
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ElectionStatus Status { get; set; } = ElectionStatus.Draft;

    public int MinTenureYears { get; set; }
    public int CreatedByUserId { get; set; }
    public bool IsLocked { get; set; } = false;

    public ICollection<CandidateApplication> CandidateApplications { get; set; } = new List<CandidateApplication>();
    public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    public ICollection<Delegation> Delegations { get; set; } = new List<Delegation>();
}
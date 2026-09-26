namespace PollAndVotingSystem.DTOs.Vote;

public class VoteStatusResponseDto
{
    public bool HasVoted { get; set; }
    public bool VotedOnBehalfByDelegate{get; set;}
    public int? CandidateId {get; set;}
    public DateTime? Timestamp {get; set;}
} 
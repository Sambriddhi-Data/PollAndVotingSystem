namespace PollAndVotingSystem.DTOs.Vote;


public class CandidateResultDto
{
    public int CandidateId{get; set;}
    public string CandidateName{get; set;} = string.Empty;
    public int VoteCount {get; set;}
}
public class ElectionResultsResponseDto
{
    public int ElectionId{get; set;}
    public string ElectionStatus{get; set;} = string.Empty;
    public int TotalVotesCast{get; set;}

    public List<CandidateResultDto> Results { get; set; } = new();
}
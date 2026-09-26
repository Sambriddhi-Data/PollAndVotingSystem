using System.ComponentModel.DataAnnotations;
namespace PollAndVotingSystem.DTOs.Vote;

public class CastVoteRequestDto
{
    [Required]
    public int CandidateId { get; set; }
}
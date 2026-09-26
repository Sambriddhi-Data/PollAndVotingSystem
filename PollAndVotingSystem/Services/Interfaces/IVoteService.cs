using PollAndVotingSystem.DTOs.Vote;
namespace PollAndVotingSystem.Services.Interfaces;

public interface IVoteService
{
    Task CastVoteAsync(int electionId, CastVoteRequestDto request);
    Task<VoteStatusResponseDto> GetMyVoteStatusAsync(int electionId);
    Task<ElectionResultsResponseDto> GetResultsAsync(int electionId);
}
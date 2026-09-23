using PollAndVotingSystem.DTOs.Candidate;
using PollAndVotingSystem.DTOs.CandidateApplication;
namespace PollAndVotingSystem.Services.Interfaces;

public interface ICandidateApplicationService
{
    Task<CandidateApplicationResponseDto> ApplyAsync(int electionId, ApplyRequestDto request);
    Task<List<CandidateApplicationResponseDto>> GetByElectionAsync(int electionId);
    Task<List<CandidateApplicationResponseDto>> GetMyApplicationsAsync();
    Task<CandidateApplicationResponseDto> ApproveAsync(int applicationId);
    Task<CandidateApplicationResponseDto> RejectAsync(int applicationId, ReviewRequestDto request);
    Task<List<CandidateResponseDto>> GetCandidatesByElectionAsync(int electionId);

}
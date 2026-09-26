using PollAndVotingSystem.DTOs.Delegation;
namespace PollAndVotingSystem.Services.Interfaces;

public interface IDelegationService
{
    Task<DelegationStatusResponseDto> CreateAsync(int electionId, CreateDelegationRequestDto request);
    Task RevokeAsync(int electionId);
    Task<DelegationStatusResponseDto> GetMyStatusAsync(int electionId);
}

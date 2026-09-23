using PollAndVotingSystem.DTOs.Election;

namespace PollAndVotingSystem.Services
{
    public interface IElectionService
    {
        Task<List<ElectionResponseDto>> GetElectionsForCurrentUserAsync();
        Task<ElectionResponseDto> GetByIdAsync(int id);
        Task<ElectionResponseDto> CreateAsync(ElectionRequestDto request);
        Task<ElectionResponseDto> UpdateAsync(int id, ElectionRequestDto request);
        
        Task<ElectionResponseDto> ActivateAsync(int id);
        Task<ElectionResponseDto> CloseAsync(int id);
        Task<ElectionResponseDto> LockOverrideAsync(int id, string reason);
        Task DeleteAsync(int id);

    }
}
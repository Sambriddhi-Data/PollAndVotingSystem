using PollAndVotingSystem.Models;
namespace PollAndVotingSystem.Repositories;

public interface IDelegationRepository
{
    Task<Delegation?> GetByDelegatorAsync(int electionId, int delegatorId);
    Task<List<Delegation>> GetByDelegateAsync(int electionId, int delegateId);
    Task<bool> IsDelegatorAsync(int electionId, int userId);
    Task AddAsync(Delegation delegation);
    void Remove(Delegation delegation);
    Task SaveChangesAsync();
}
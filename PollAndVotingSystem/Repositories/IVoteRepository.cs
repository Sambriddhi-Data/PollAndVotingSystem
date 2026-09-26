using PollAndVotingSystem.Models;
namespace PollAndVotingSystem.Repositories;

public interface IVoteRepository
{
    Task<bool> HasVotedAsync(int electionId, int voterId);

    Task<Vote?> GetByElectionAndVoterAsync(int electionId, int voterId);

    Task<Candidate?> GetCandidateAsync(int candidateId, int electionId);

    Task AddAsync(Vote vote);

    Task AddRangeAsync(IEnumerable<Vote> votes);

    Task<List<Vote>> GetByElectionAsync(int electionId);

    Task SaveChangesAsync();
}
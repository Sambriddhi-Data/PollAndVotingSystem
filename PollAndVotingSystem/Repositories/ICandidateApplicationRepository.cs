using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Repositories;
    public interface ICandidateApplicationRepository
    {
        Task<List<CandidateApplication>> GetByElectionAsync(int electionId);
        Task<List<CandidateApplication>> GetByUserAsync(int userId);
        Task<CandidateApplication?> GetByIdAsync(int id);
        Task<bool> HasAppliedAsync(int electionId, int userId);
        Task AddAsync(CandidateApplication application);
        Task AddCandidateAsync(Candidate candidate);
        Task<List<Models.Candidate>> GetCandidatesByElectionAsync(int electionId);
        Task SaveChangesAsync();
    }

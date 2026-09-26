using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Repositories
{
    public interface IElectionRepository
    {
        Task<List<Election>> GetByDepartmentAsync(int departmentId);
        Task<Election?> GetByIdAsync(int id);
        Task<Election?> GetByIdWithDepartmentAsync(int id);
        Task AddAsync(Election election);
        void Update(Election election);
        void Remove(Election election);
        Task<List<Election>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
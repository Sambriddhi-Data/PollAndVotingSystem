using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
    }
}
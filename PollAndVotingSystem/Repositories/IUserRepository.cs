using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Repositories;
public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> DepartmentExistsAsync(int departmentId);
        Task<List<User>> GetByDepartmentAsync(int departmentId);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }

using Microsoft.EntityFrameworkCore;
using PollAndVotingSystem.Data;
using PollAndVotingSystem.Models;
using PollAndVotingSystem.Repositories;

namespace PollAndVotingSystem.Repositories;

public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<List<User>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.Users
                .Where(u => u.DepartmentId == departmentId)
                .ToListAsync();
        }
        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            return await _context.Departments.AnyAsync(d => d.Id == departmentId);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

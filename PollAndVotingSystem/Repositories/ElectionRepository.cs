using Microsoft.EntityFrameworkCore;
using PollAndVotingSystem.Data;
using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Repositories
{
    public class ElectionRepository : IElectionRepository
    {
        private readonly ApplicationDbContext _context;

        public ElectionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Election>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.Elections
                .Include(e => e.Department)
                .Where(e => e.DepartmentId == departmentId)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<Election?> GetByIdAsync(int id)
        {
            return await _context.Elections.FindAsync(id);
        }

        public async Task<Election?> GetByIdWithDepartmentAsync(int id)
        {
            return await _context.Elections
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Election election)
        {
            await _context.Elections.AddAsync(election);
        }

        public void Update(Election election)
        {
            _context.Elections.Update(election);
        }

        public void Remove(Election election)
        {
            _context.Elections.Remove(election);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
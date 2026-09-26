using Microsoft.EntityFrameworkCore;
using PollAndVotingSystem.Data;
using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Repositories;

public class DelegationRepository : IDelegationRepository
    {
        private readonly ApplicationDbContext _context;

        public DelegationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Delegation?> GetByDelegatorAsync(int electionId, int delegatorId)
        {
            return await _context.Delegations
                .Include(d => d.Delegate)
                .FirstOrDefaultAsync(d => d.ElectionId == electionId && d.DelegatorId == delegatorId);
        }

        public async Task<List<Delegation>> GetByDelegateAsync(int electionId, int delegateId)
        {
            return await _context.Delegations
                .Where(d => d.ElectionId == electionId && d.DelegateId == delegateId)
                .ToListAsync();
        }

        public async Task<bool> IsDelegatorAsync(int electionId, int userId)
        {
            // True if this user has already delegated their own vote away in this election
            return await _context.Delegations
                .AnyAsync(d => d.ElectionId == electionId && d.DelegatorId == userId);
        }

        public async Task AddAsync(Delegation delegation)
        {
            await _context.Delegations.AddAsync(delegation);
        }

        public void Remove(Delegation delegation)
        {
            _context.Delegations.Remove(delegation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

using Microsoft.EntityFrameworkCore;
using PollAndVotingSystem.Data;
using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Repositories;
    public class CandidateApplicationRepository : ICandidateApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public CandidateApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CandidateApplication>> GetByElectionAsync(int electionId)
        {
            return await _context.CandidateApplications
                .Include(c => c.User)
                .Where(c => c.ElectionId == electionId)
                .ToListAsync();
        }

        public async Task<List<CandidateApplication>> GetByUserAsync(int userId)
        {
            return await _context.CandidateApplications
                .Include(c => c.Election)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<CandidateApplication?> GetByIdAsync(int id)
        {
            return await _context.CandidateApplications
                .Include(c => c.User)
                .Include(c => c.Election)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> HasAppliedAsync(int electionId, int userId)
        {
            return await _context.CandidateApplications
                .AnyAsync(c => c.ElectionId == electionId && c.UserId == userId);
        }

        public async Task AddAsync(CandidateApplication application)
        {
            await _context.CandidateApplications.AddAsync(application);
        }

        public async Task AddCandidateAsync(Candidate candidate)
        {
            await _context.Candidates.AddAsync(candidate);
        }

        public async Task<List<Candidate>> GetCandidatesByElectionAsync(int electionId)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .Include(c=> c.Application)
                .Where(c => c.ElectionId == electionId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
}
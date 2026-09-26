using Microsoft.EntityFrameworkCore;
using PollAndVotingSystem.Data;
using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly ApplicationDbContext _context;

        public VoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasVotedAsync(int electionId, int voterId)
        {
            return await _context.Votes
                .AnyAsync(v => v.ElectionId == electionId && v.VoterId == voterId);
        }

        public async Task<Vote?> GetByElectionAndVoterAsync(int electionId, int voterId)
        {
            return await _context.Votes
                .FirstOrDefaultAsync(v => v.ElectionId == electionId && v.VoterId == voterId);
        }

        public async Task<Candidate?> GetCandidateAsync(int candidateId, int electionId)
        {
            return await _context.Candidates
                .FirstOrDefaultAsync(c => c.Id == candidateId && c.ElectionId == electionId);
        }

        public async Task AddAsync(Vote vote)
        {
            await _context.Votes.AddAsync(vote);
        }

        public async Task AddRangeAsync(IEnumerable<Vote> votes)
        {
            await _context.Votes.AddRangeAsync(votes);
        }

        public async Task<List<Vote>> GetByElectionAsync(int electionId)
        {
            return await _context.Votes
                .Include(v => v.Candidate)
                .ThenInclude(c => c!.User)
                .Where(v => v.ElectionId == electionId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

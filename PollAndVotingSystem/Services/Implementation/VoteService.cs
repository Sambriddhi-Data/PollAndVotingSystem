using Microsoft.AspNetCore.SignalR;
using PollAndVotingSystem.Common;
using PollAndVotingSystem.DTOs.Vote;
using PollAndVotingSystem.Models;
using PollAndVotingSystem.Repositories;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Services.Implementation
{
    public class VoteService : Interfaces.IVoteService
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IDelegationRepository _delegationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IAuditLogService _auditLogService;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<PollAndVotingSystem.Hubs.ElectionResultsHub> _hubContext;

        public VoteService(
            IVoteRepository voteRepository,
            IElectionRepository electionRepository,
            IDelegationRepository delegationRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUser,
            IAuditLogService auditLogService,
            Microsoft.AspNetCore.SignalR.IHubContext<PollAndVotingSystem.Hubs.ElectionResultsHub> hubContext)
        {
            _voteRepository = voteRepository;
            _electionRepository = electionRepository;
            _delegationRepository = delegationRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _auditLogService = auditLogService;
            _hubContext = hubContext;
        }
        
        private async Task<PollAndVotingSystem.DTOs.Vote.ElectionResultsResponseDto> GetResultsForBroadcastAsync(int electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            var votes = await _voteRepository.GetByElectionAsync(electionId);

            var results = votes
                .GroupBy(v => new { v.CandidateId, Name = v.Candidate?.User?.Name ?? "Unknown" })
                .Select(g => new PollAndVotingSystem.DTOs.Vote.CandidateResultDto
                {
                    CandidateId = g.Key.CandidateId,
                    CandidateName = g.Key.Name,
                    VoteCount = g.Count()
                })
                .OrderByDescending(r => r.VoteCount)
                .ToList();

            return new PollAndVotingSystem.DTOs.Vote.ElectionResultsResponseDto
            {
                ElectionId = electionId,
                ElectionStatus = election?.Status.ToString() ?? "Unknown",
                TotalVotesCast = votes.Count,
                Results = results
            };
        }

        public async Task CastVoteAsync(int electionId, CastVoteRequestDto request)
        {
            var election = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new KeyNotFoundException("Election not found.");

            // Server-verified: election must be Active to vote
            if (election.Status != ElectionStatus.Active)
                throw new InvalidOperationException("Voting is only allowed while the election is Active.");

            // Server-verified: correct department/scope
            if (election.DepartmentId != _currentUser.DepartmentId)
                throw new UnauthorizedAccessException("You cannot vote in an election outside your department.");

            var voter = await _userRepository.GetByIdAsync(_currentUser.UserId)
                ?? throw new UnauthorizedAccessException("User not found.");

            var tenureYears = (DateTime.UtcNow - voter.DateJoined).TotalDays / 365.25;
            if (tenureYears < election.MinTenureYears)
                throw new InvalidOperationException("You do not meet the eligibility requirements to vote.");

            // Server-verified: no prior vote in this election
            if (await _voteRepository.HasVotedAsync(electionId, _currentUser.UserId))
                throw new InvalidOperationException("You have already voted in this election.");

            // Rule: a voter who delegated cannot also cast a direct vote
            if (await _delegationRepository.IsDelegatorAsync(electionId, _currentUser.UserId))
                throw new InvalidOperationException("You have delegated your vote and cannot vote directly in this election.");

            var candidate = await _voteRepository.GetCandidateAsync(request.CandidateId, electionId)
                ?? throw new InvalidOperationException("Candidate not found in this election.");

            var votesToAdd = new List<Vote>
            {
                new Vote
                {
                    ElectionId = electionId,
                    CandidateId = candidate.Id,
                    VoterId = _currentUser.UserId
                }
            };

            // If others delegated their vote to this voter, cast on their behalf too (single-hop only)
            var receivedDelegations = await _delegationRepository.GetByDelegateAsync(electionId, _currentUser.UserId);

            foreach (var delegation in receivedDelegations)
            {
                // Safety check: a delegator should never already have a vote row, but guard anyway
                if (!await _voteRepository.HasVotedAsync(electionId, delegation.DelegatorId))
                {
                    votesToAdd.Add(new Vote
                    {
                        ElectionId = electionId,
                        CandidateId = candidate.Id,
                        VoterId = delegation.DelegatorId
                    });
                }
            }

            await _voteRepository.AddRangeAsync(votesToAdd);
            
            await _voteRepository.SaveChangesAsync();
            await _auditLogService.LogAsync(
                _currentUser.UserId, "VoteCast", "Election", electionId,
                $"Voted for CandidateId {candidate.Id}; {votesToAdd.Count} vote(s) recorded including delegations.");
           
            // Push live tally update to any Admins watching this election
            var liveResults = await GetResultsForBroadcastAsync(electionId);
            await _hubContext.Clients
                .Group(PollAndVotingSystem.Hubs.ElectionResultsHub.GroupName(electionId))
                .SendAsync("ResultsUpdated", liveResults);
        }

        public async Task<VoteStatusResponseDto> GetMyVoteStatusAsync(int electionId)
        {
            var vote = await _voteRepository.GetByElectionAndVoterAsync(electionId, _currentUser.UserId);

            if (vote == null)
            {
                return new VoteStatusResponseDto { HasVoted = false };
            }

            // If this user delegated, their vote row was inserted by their delegate, not by them directly
            var delegated = await _delegationRepository.IsDelegatorAsync(electionId, _currentUser.UserId);

            return new VoteStatusResponseDto
            {
                HasVoted = true,
                VotedOnBehalfByDelegate = delegated,
                CandidateId = vote.CandidateId,
                Timestamp = vote.Timestamp
            };
        }

        public async Task<ElectionResultsResponseDto> GetResultsAsync(int electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new KeyNotFoundException("Election not found.");

            if (election.DepartmentId != _currentUser.DepartmentId)
                throw new UnauthorizedAccessException("You cannot view results for elections outside your department.");

            // Rule: results hidden from voters until Closed; Admin can see live tally while Active
            var isAdmin = _currentUser.Role == Roles.Admin;
            if (election.Status != ElectionStatus.Closed && !isAdmin)
                throw new InvalidOperationException("Results are not available until the election is closed.");

            var votes = await _voteRepository.GetByElectionAsync(electionId);

            var results = votes
                .GroupBy(v => new { v.CandidateId, Name = v.Candidate?.User?.Name ?? "Unknown" })
                .Select(g => new CandidateResultDto
                {
                    CandidateId = g.Key.CandidateId,
                    CandidateName = g.Key.Name,
                    VoteCount = g.Count()
                })
                .OrderByDescending(r => r.VoteCount)
                .ToList();

            return new ElectionResultsResponseDto
            {
                ElectionId = electionId,
                ElectionStatus = election.Status.ToString(),
                TotalVotesCast = votes.Count,
                Results = results
            };
        }
    }
}
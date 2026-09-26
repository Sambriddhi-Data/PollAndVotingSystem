using PollAndVotingSystem.Common;
using PollAndVotingSystem.DTOs.Delegation;
using PollAndVotingSystem.Models;
using PollAndVotingSystem.Repositories;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Services.Implementation;

public class DelegationService : Interfaces.IDelegationService
{
        private readonly IDelegationRepository _delegationRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IAuditLogService  _auditLogService;

        public DelegationService(
            IDelegationRepository delegationRepository,
            IElectionRepository electionRepository,
            IVoteRepository voteRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUser,
            IAuditLogService auditLogService)
        {
            _delegationRepository = delegationRepository;
            _electionRepository = electionRepository;
            _voteRepository = voteRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _auditLogService = auditLogService; 
        }

        public async Task<DelegationStatusResponseDto> CreateAsync(int electionId, CreateDelegationRequestDto request)
        {
            var election = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new KeyNotFoundException("Election not found.");

            if (election.Status != ElectionStatus.Active)
                throw new InvalidOperationException("Delegation is only allowed while the election is Active.");

            if (election.DepartmentId != _currentUser.DepartmentId)
                throw new UnauthorizedAccessException("You cannot delegate in an election outside your department.");

            if (request.DelegateId == _currentUser.UserId)
                throw new InvalidOperationException("You cannot delegate your vote to yourself.");

            // Rule: delegator cannot also vote directly -> if they've already voted, no delegation allowed
            if (await _voteRepository.HasVotedAsync(electionId, _currentUser.UserId))
                throw new InvalidOperationException("You have already voted directly and cannot delegate your vote.");

            // Rule: one delegation per delegator per election (also enforced by unique DB index)
            if (await _delegationRepository.GetByDelegatorAsync(electionId, _currentUser.UserId) != null)
                throw new InvalidOperationException("You have already delegated your vote in this election.");

            // Rule: single-hop only — a user who has already received a delegation cannot delegate onward
            var receivedDelegations = await _delegationRepository.GetByDelegateAsync(electionId, _currentUser.UserId);
            if (receivedDelegations.Any())
                throw new InvalidOperationException("You have received a delegated vote and cannot delegate your own vote onward.");
            
            // Rule: single-hop only — the chosen delegate must not themselves have delegated onward
            if (await _delegationRepository.IsDelegatorAsync(electionId, request.DelegateId))
                throw new InvalidOperationException("The selected delegate has already delegated their own vote and cannot receive delegations.");

            // Delegate must be an eligible voter in the same department/election
            var delegateUser = await _userRepository.GetByIdAsync(request.DelegateId)
                ?? throw new InvalidOperationException("Delegate user not found.");

            if (delegateUser.DepartmentId != election.DepartmentId)
                throw new InvalidOperationException("Delegate must belong to the same department as the election.");

            var delegateTenureYears = (DateTime.UtcNow - delegateUser.DateJoined).TotalDays / 365.25;
            if (delegateTenureYears < election.MinTenureYears)
                throw new InvalidOperationException("Delegate does not meet the eligibility requirements for this election.");

            var delegation = new Delegation
            {
                ElectionId = electionId,
                DelegatorId = _currentUser.UserId,
                DelegateId = request.DelegateId
            };

            await _delegationRepository.AddAsync(delegation);
            await _delegationRepository.SaveChangesAsync();
            
            await _auditLogService.LogAsync(
                _currentUser.UserId, "DelegationCreated", "Election", electionId,
                $"Delegated to UserId {request.DelegateId}");

            return new DelegationStatusResponseDto
            {
                HasDelegated = true,
                DelegateId = delegateUser.Id,
                DelegateName = delegateUser.Name,
                CreatedAt = delegation.CreatedAt
            };
        }

        public async Task RevokeAsync(int electionId)
        {
            var delegation = await _delegationRepository.GetByDelegatorAsync(electionId, _currentUser.UserId)
                ?? throw new KeyNotFoundException("No delegation found to revoke.");

            // If the delegate already voted on the delegator's behalf, a Vote row exists for the delegator.
            // Revoking after that point would silently orphan/mismatch that vote, so block it.
            if (await _voteRepository.HasVotedAsync(electionId, _currentUser.UserId))
                throw new InvalidOperationException("Your delegate has already voted on your behalf; delegation can no longer be revoked.");

            _delegationRepository.Remove(delegation);
            await _delegationRepository.SaveChangesAsync();
        }

        public async Task<DelegationStatusResponseDto> GetMyStatusAsync(int electionId)
        {
            var delegation = await _delegationRepository.GetByDelegatorAsync(electionId, _currentUser.UserId);

            if (delegation == null)
            {
                return new DelegationStatusResponseDto { HasDelegated = false };
            }

            return new DelegationStatusResponseDto
            {
                HasDelegated = true,
                DelegateId = delegation.DelegateId,
                DelegateName = delegation.Delegate?.Name,
                CreatedAt = delegation.CreatedAt
            };
        }
}
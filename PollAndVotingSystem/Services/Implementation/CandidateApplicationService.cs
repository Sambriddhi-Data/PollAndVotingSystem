using PollAndVotingSystem.Common;
using PollAndVotingSystem.DTOs.Candidate;
using PollAndVotingSystem.DTOs.CandidateApplication;
using PollAndVotingSystem.Models;
using PollAndVotingSystem.Repositories;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Services.Implementation;
    public class CandidateApplicationService : ICandidateApplicationService
    {
        private readonly ICandidateApplicationRepository _appRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;

        private readonly IAuditLogService _auditLogService;
        public CandidateApplicationService(
            ICandidateApplicationRepository appRepository,
            IElectionRepository electionRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUser,
            IAuditLogService auditLogService)
        {
            _appRepository = appRepository;
            _electionRepository = electionRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _auditLogService = auditLogService;
        }

        public async Task<CandidateApplicationResponseDto> ApplyAsync(int electionId, ApplyRequestDto request)
        {
            var election = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new KeyNotFoundException("Election not found.");

            // Rule: applications only accepted while Draft
            if (election.Status != ElectionStatus.Draft)
                throw new InvalidOperationException("Applications are only accepted while the election is in Draft status.");

            // Rule: same department only
            if (election.DepartmentId != _currentUser.DepartmentId)
                throw new UnauthorizedAccessException("You can only apply to elections in your own department.");

            // Rule: eligibility = tenure
            var user = await _userRepository.GetByIdAsync(_currentUser.UserId)
                ?? throw new UnauthorizedAccessException("User not found.");

            var tenureYears = (DateTime.UtcNow - user.DateJoined).TotalDays / 365.25;
            if (tenureYears < election.MinTenureYears)
                throw new InvalidOperationException(
                    $"You do not meet the minimum tenure requirement of {election.MinTenureYears} year(s).");

            if (await _appRepository.HasAppliedAsync(electionId, _currentUser.UserId))
                throw new InvalidOperationException("You have already applied to this election.");

            var application = new CandidateApplication
            {
                ElectionId = electionId,
                UserId = _currentUser.UserId,
                Statement = request.Statement,
                Status = ApplicationStatus.Pending
            };

            await _appRepository.AddAsync(application);
            await _appRepository.SaveChangesAsync();

            application.User = user;
            return MapToDto(application);
        }

        public async Task<List<CandidateApplicationResponseDto>> GetByElectionAsync(int electionId)
        {
            var apps = await _appRepository.GetByElectionAsync(electionId);
            return apps.Select(MapToDto).ToList();
        }

        public async Task<List<CandidateApplicationResponseDto>> GetMyApplicationsAsync()
        {
            var apps = await _appRepository.GetByUserAsync(_currentUser.UserId);
            return apps.Select(MapToDto).ToList();
        }

        public async Task<CandidateApplicationResponseDto> ApproveAsync(int applicationId)
        {
            var application = await _appRepository.GetByIdAsync(applicationId)
                ?? throw new KeyNotFoundException("Application not found.");

            if (application.Status != ApplicationStatus.Pending)
                throw new InvalidOperationException("Only pending applications can be approved.");

            application.Status = ApplicationStatus.Approved;
            application.ReviewedByUserId = _currentUser.UserId;
            application.ReviewedAt = DateTime.UtcNow;

            var candidate = new Candidate
            {
                ElectionId = application.ElectionId,
                UserId = application.UserId,
                ApplicationId = application.Id
            };

            await _appRepository.AddCandidateAsync(candidate);
            await _appRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                _currentUser.UserId, "CandidateApplicationApproved", "CandidateApplication", application.Id);
            return MapToDto(application);
        }

        public async Task<CandidateApplicationResponseDto> RejectAsync(int applicationId, ReviewRequestDto request)
        {
            var application = await _appRepository.GetByIdAsync(applicationId)
                ?? throw new KeyNotFoundException("Application not found.");

            if (application.Status != ApplicationStatus.Pending)
                throw new InvalidOperationException("Only pending applications can be rejected.");

            application.Status = ApplicationStatus.Rejected;
            application.ReviewedByUserId = _currentUser.UserId;
            application.ReviewedAt = DateTime.UtcNow;
            application.RejectionReason = request.RejectionReason;

            await _appRepository.SaveChangesAsync();
            await _auditLogService.LogAsync(
                _currentUser.UserId, "CandidateApplicationRejected", "CandidateApplication", application.Id, request.RejectionReason);
            return MapToDto(application);
        }

        public async Task<List<CandidateResponseDto>> GetCandidatesByElectionAsync(int electionId)
        {
            var candidates = await _appRepository.GetCandidatesByElectionAsync(electionId);
            return candidates.Select(c => new CandidateResponseDto
            {
                Id = c.Id,
                ElectionId = c.ElectionId,
                UserId = c.UserId,
                UserName = c.User?.Name ?? string.Empty,
                Statement = c.Application?.Statement ?? string.Empty
            }).ToList();
        }

        private static CandidateApplicationResponseDto MapToDto(CandidateApplication a) => new()
        {
            Id = a.Id,
            ElectionId = a.ElectionId,
            UserId = a.UserId,
            UserName = a.User?.Name ?? string.Empty,
            Statement = a.Statement,
            Status = a.Status.ToString(),
            ReviewedByUserId = a.ReviewedByUserId,
            ReviewedAt = a.ReviewedAt,
            RejectionReason = a.RejectionReason
        };
    }

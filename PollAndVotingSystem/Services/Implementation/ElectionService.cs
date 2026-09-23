using PollAndVotingSystem.Common;
using PollAndVotingSystem.DTOs.Election;
using PollAndVotingSystem.Models;
using PollAndVotingSystem.Repositories;

namespace PollAndVotingSystem.Services
{
    public class ElectionService : IElectionService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICurrentUserService _currentUser;

        public ElectionService(
            IElectionRepository electionRepository,
            IDepartmentRepository departmentRepository,
            ICurrentUserService currentUser)
        {
            _electionRepository = electionRepository;
            _departmentRepository = departmentRepository;
            _currentUser = currentUser;
        }

        public async Task<List<ElectionResponseDto>> GetElectionsForCurrentUserAsync()
        {
            // Department-scoped visibility: voters only see their own department's elections
            var elections = await _electionRepository.GetByDepartmentAsync(_currentUser.DepartmentId);
            return elections.Select(MapToDto).ToList();
        }

        public async Task<ElectionResponseDto> GetByIdAsync(int id)
        {
            var election = await _electionRepository.GetByIdWithDepartmentAsync(id)
                ?? throw new KeyNotFoundException("Election not found.");

            if (election.DepartmentId != _currentUser.DepartmentId)
                throw new UnauthorizedAccessException("You cannot view elections outside your department.");

            return MapToDto(election);
        }

        public async Task<ElectionResponseDto> CreateAsync(ElectionRequestDto request)
        {
            if (request.EndDate <= request.StartDate)
                throw new InvalidOperationException("EndDate must be after StartDate.");

            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId)
                ?? throw new InvalidOperationException("Department does not exist.");

            var election = new Election
            {
                Title = request.Title,
                Description = request.Description,
                DepartmentId = request.DepartmentId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MinTenureYears = request.MinTenureYears,
                Status = ElectionStatus.Draft,
                IsLocked = false,
                CreatedByUserId = _currentUser.UserId
            };

            await _electionRepository.AddAsync(election);
            await _electionRepository.SaveChangesAsync();

            election.Department = department;
            return MapToDto(election);
        }

        public async Task<ElectionResponseDto> UpdateAsync(int id, ElectionRequestDto request)
        {
            var election = await _electionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Election not found.");

            // Election locking rule: once Active, candidates/eligibility become immutable via normal update
            if (election.IsLocked)
                throw new InvalidOperationException("Election is locked. Use the override endpoint to make changes.");

            if (request.EndDate <= request.StartDate)
                throw new InvalidOperationException("EndDate must be after StartDate.");

            election.Title = request.Title;
            election.Description = request.Description;
            election.StartDate = request.StartDate;
            election.EndDate = request.EndDate;
            election.MinTenureYears = request.MinTenureYears;
            // DepartmentId intentionally not editable after creation to avoid orphaning applications/votes

            _electionRepository.Update(election);
            await _electionRepository.SaveChangesAsync();

            var withDept = await _electionRepository.GetByIdWithDepartmentAsync(id);
            return MapToDto(withDept!);
        }

        public async Task DeleteAsync(int id)
        {
            var election = await _electionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Election not found.");

            if (election.Status != ElectionStatus.Draft)
                throw new InvalidOperationException("Only Draft elections can be deleted.");

            _electionRepository.Remove(election);
            await _electionRepository.SaveChangesAsync();
        }

        private static ElectionResponseDto MapToDto(Election e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.Name ?? string.Empty,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Status = e.Status.ToString(),
            MinTenureYears = e.MinTenureYears,
            IsLocked = e.IsLocked,
            CreatedByUserId = e.CreatedByUserId
        };
        
        public async Task<ElectionResponseDto> ActivateAsync(int id)
        {
            var election = await _electionRepository.GetByIdWithDepartmentAsync(id)
                ?? throw new KeyNotFoundException("Election not found.");

            if (election.Status != ElectionStatus.Draft)
                throw new InvalidOperationException("Only Draft elections can be activated.");

            election.Status = ElectionStatus.Active;
            election.IsLocked = true; // Rule: once Active, candidates/eligibility become locked

            _electionRepository.Update(election);
            await _electionRepository.SaveChangesAsync();

            return MapToDto(election);
        }

        public async Task<ElectionResponseDto> CloseAsync(int id)
        {
            var election = await _electionRepository.GetByIdWithDepartmentAsync(id)
                ?? throw new KeyNotFoundException("Election not found.");

            if (election.Status != ElectionStatus.Active)
                throw new InvalidOperationException("Only Active elections can be closed.");

            election.Status = ElectionStatus.Closed;

            _electionRepository.Update(election);
            await _electionRepository.SaveChangesAsync();

            return MapToDto(election);
        }

        public async Task<ElectionResponseDto> LockOverrideAsync(int id, string reason)
        {
            var election = await _electionRepository.GetByIdWithDepartmentAsync(id)
                ?? throw new KeyNotFoundException("Election not found.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new InvalidOperationException("A reason is required to override the lock.");

            // Temporarily unlock so Admin can make one authorized change via the normal Update endpoint
            election.IsLocked = false;

            _electionRepository.Update(election);
            await _electionRepository.SaveChangesAsync();

            return MapToDto(election);
        }
    }
}
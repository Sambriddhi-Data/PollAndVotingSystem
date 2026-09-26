using PollAndVotingSystem.DTOs.AuditLog;
using PollAndVotingSystem.Models;
using PollAndVotingSystem.Repositories;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Services.Implementation
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUserRepository _userRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository, IUserRepository userRepository)
        {
            _auditLogRepository = auditLogRepository;
            _userRepository = userRepository;
        }

        public async Task LogAsync(int userId, string action, string entityType, int entityId, string? details = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Details = details
            };

            await _auditLogRepository.AddAsync(log);
            await _auditLogRepository.SaveChangesAsync();
        }

        public async Task<List<AuditLogResponseDto>> GetLogsAsync(string? entityType, int? entityId)
        {
            var logs = await _auditLogRepository.GetAllAsync(entityType, entityId);
            var result = new List<AuditLogResponseDto>();

            foreach (var log in logs)
            {
                var user = await _userRepository.GetByIdAsync(log.UserId);
                result.Add(new AuditLogResponseDto
                {
                    Id = log.Id,
                    UserId = log.UserId,
                    UserName = user?.Name ?? "Unknown",
                    Action = log.Action,
                    EntityType = log.EntityType,
                    EntityId = log.EntityId,
                    Timestamp = log.Timestamp,
                    Details = log.Details
                });
            }

            return result;
        }
    }
}
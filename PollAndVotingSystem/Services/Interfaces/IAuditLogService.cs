namespace PollAndVotingSystem.Services.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(int userId, string action, string entityType, int entityId, string? details = null);
    Task<List<PollAndVotingSystem.DTOs.AuditLog.AuditLogResponseDto>> GetLogsAsync(string? entityType, int? entityId);

}
using PollAndVotingSystem.Models;
namespace PollAndVotingSystem.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);

    Task<List<AuditLog>> GetAllAsync(string? entityType, int? entityId);
    Task SaveChangesAsync();
}
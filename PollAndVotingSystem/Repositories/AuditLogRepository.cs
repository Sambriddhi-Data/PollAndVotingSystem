using Microsoft.EntityFrameworkCore;
using PollAndVotingSystem.Data;
using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
        }

        public async Task<List<AuditLog>> GetAllAsync(string? entityType, int? entityId)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityType))
                query = query.Where(a => a.EntityType == entityType);

            if (entityId.HasValue)
                query = query.Where(a => a.EntityId == entityId.Value);

            return await query.OrderByDescending(a => a.Timestamp).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
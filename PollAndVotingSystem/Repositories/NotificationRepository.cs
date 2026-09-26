using Microsoft.EntityFrameworkCore;
using PollAndVotingSystem.Data;
using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task AddRangeAsync(IEnumerable<Notification> notifications)
        {
            await _context.Notifications.AddRangeAsync(notifications);
        }

        public async Task<List<Notification>> GetByUserAsync(int userId)
        {
            return await _context.Notifications
                .Include(n => n.Election)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }
        
        public async Task<bool> HasNotificationTypeBeenSentAsync(int electionId, NotificationType type)
        {
            return await _context.Notifications
                .AnyAsync(n => n.ElectionId == electionId && n.Type == type);
        }

        public async Task<List<int>> GetUserIdsByDepartmentAsync(int departmentId)
        {
            return await _context.Users
                .Where(u => u.DepartmentId == departmentId)
                .Select(u => u.Id)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
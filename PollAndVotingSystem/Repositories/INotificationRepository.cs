using PollAndVotingSystem.Models;
namespace PollAndVotingSystem.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task AddRangeAsync(IEnumerable<Notification> notifications);
    Task<List<Notification>> GetByUserAsync(int userId);
    Task<Notification?> GetByIdAsync(int id);
    Task<List<int>> GetUserIdsByDepartmentAsync(int departmentId);
    Task<bool> HasNotificationTypeBeenSentAsync(int electionId, Models.NotificationType type);
    Task SaveChangesAsync();
}
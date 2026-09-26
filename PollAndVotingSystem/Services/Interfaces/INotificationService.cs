using PollAndVotingSystem.DTOs.Notification;
namespace PollAndVotingSystem.Services.Interfaces;

public interface INotificationService
{
    Task<List<NotificationResponseDto>> GetMyNotificationsAsync();
    Task MarkAsReadAsync(int notificationId);
    Task NotifyDepartmentAsync(int electionId, int departmentId, string type, string message);

}
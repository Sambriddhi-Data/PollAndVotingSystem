using PollAndVotingSystem.Common;
using PollAndVotingSystem.DTOs.Notification;
using PollAndVotingSystem.Models;
using PollAndVotingSystem.Repositories;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Services.Implementation;
public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUser;

        public NotificationService(INotificationRepository notificationRepository, ICurrentUserService currentUser)
        {
            _notificationRepository = notificationRepository;
            _currentUser = currentUser;
        }

        public async Task<List<NotificationResponseDto>> GetMyNotificationsAsync()
        {
            var notifications = await _notificationRepository.GetByUserAsync(_currentUser.UserId);

            return notifications.Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                ElectionId = n.ElectionId,
                ElectionTitle = n.Election?.Title ?? string.Empty,
                Type = n.Type.ToString(),
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId)
                ?? throw new KeyNotFoundException("Notification not found.");

            if (notification.UserId != _currentUser.UserId)
                throw new UnauthorizedAccessException("You cannot modify another user's notification.");

            notification.IsRead = true;
            await _notificationRepository.SaveChangesAsync();
        }

        public async Task NotifyDepartmentAsync(int electionId, int departmentId, string type, string message)
        {
            if (!Enum.TryParse<NotificationType>(type, out var notificationType))
                throw new InvalidOperationException("Invalid notification type.");

            var userIds = await _notificationRepository.GetUserIdsByDepartmentAsync(departmentId);

            var notifications = userIds.Select(userId => new Notification
            {
                UserId = userId,
                ElectionId = electionId,
                Type = notificationType,
                Message = message
            });

            await _notificationRepository.AddRangeAsync(notifications);
            await _notificationRepository.SaveChangesAsync();
        }
    }

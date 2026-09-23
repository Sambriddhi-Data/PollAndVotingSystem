namespace PollAndVotingSystem.Models;

public enum NotificationType
{
    Approaching,
    Started,
    ClosingSoon
}

public class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int ElectionId { get; set; }
    public Election? Election { get; set; }

    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
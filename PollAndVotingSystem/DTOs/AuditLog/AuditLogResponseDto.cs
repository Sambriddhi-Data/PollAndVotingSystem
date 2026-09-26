namespace PollAndVotingSystem.DTOs.AuditLog;

public class AuditLogResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}
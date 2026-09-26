namespace PollAndVotingSystem.DTOs.Delegation;

public class DelegationStatusResponseDto
{
    public bool HasDelegated { get; set; }
    public int? DelegateId { get; set; }
    public string? DelegateName { get; set; }
    public DateTime? CreatedAt { get; set; }
}
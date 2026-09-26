using System.ComponentModel.DataAnnotations;
namespace PollAndVotingSystem.DTOs.Delegation;

public class CreateDelegationRequestDto
{
    [Required]
    public int DelegateId { get; set; }
}
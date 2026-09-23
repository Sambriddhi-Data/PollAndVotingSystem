using System.ComponentModel.DataAnnotations;

namespace PollAndVotingSystem.DTOs.CandidateApplication;

public class ApplyRequestDto
{
    [Required, MinLength(10)]
    public string Statement { get; set; } = string.Empty;
}
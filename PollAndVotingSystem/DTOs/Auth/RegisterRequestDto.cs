namespace PollAndVotingSystem.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

public class RegisterRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public DateTime DateJoined { get; set; }
    }

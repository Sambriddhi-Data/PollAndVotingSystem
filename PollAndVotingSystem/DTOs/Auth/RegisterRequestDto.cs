using PollAndVotingSystem.Common;
namespace PollAndVotingSystem.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

public class RegisterRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        [RegularExpression(AppConstants.EmailPattern, ErrorMessage = "Email must be a @ktmtech.com address.")]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public DateTime DateJoined { get; set; }
    }

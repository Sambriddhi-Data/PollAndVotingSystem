using System.ComponentModel.DataAnnotations;

namespace PollAndVotingSystem.DTOs.Election
{
    public class ElectionRequestDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(0, 50)]
        public int MinTenureYears { get; set; }
    }
}
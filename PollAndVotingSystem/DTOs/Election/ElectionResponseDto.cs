namespace PollAndVotingSystem.DTOs.Election
{
    public class ElectionResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int MinTenureYears { get; set; }
        public bool IsLocked { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
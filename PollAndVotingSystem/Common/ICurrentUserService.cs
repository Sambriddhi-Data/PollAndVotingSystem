namespace PollAndVotingSystem.Common
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string Role { get; }
        int DepartmentId { get; }
        bool IsAdmin { get; }
    }
}
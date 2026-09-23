namespace PollAndVotingSystem.Models;
    public enum UserRole
    {
        Voter,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Voter;

        public int DepartmentId { get; set; }
        
        public int TokenVersion { get; set; } = 1;
        public Department? Department { get; set; }

        public DateTime DateJoined { get; set; }

        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Delegation> DelegationsGiven { get; set; } = new List<Delegation>();
        public ICollection<Delegation> DelegationsReceived { get; set; } = new List<Delegation>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

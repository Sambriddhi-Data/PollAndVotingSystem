using PollAndVotingSystem.Models;
namespace PollAndVotingSystem.Authentication;

public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }

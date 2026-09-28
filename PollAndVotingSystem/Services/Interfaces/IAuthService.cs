using PollAndVotingSystem.DTOs.Auth;

namespace PollAndVotingSystem.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    Task<string?> ForgotPasswordAsync(string email);
    Task ResetPasswordAsync(ResetPasswordRequestDto request);
}
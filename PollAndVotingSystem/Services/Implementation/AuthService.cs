using Microsoft.AspNetCore.Identity;
using PollAndVotingSystem.Authentication;
using PollAndVotingSystem.Common;
using PollAndVotingSystem.DTOs.Auth;
using PollAndVotingSystem.Models;
using PollAndVotingSystem.Repositories;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Services.Implementation;
public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly PasswordHasher<User> _passwordHasher = new();
        private readonly ILogger<AuthService> _logger;
        private readonly IWebHostEnvironment _env;
        
        public AuthService(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService,
            ILogger<AuthService> logger,
            IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _env = env;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            if (!request.Email.EndsWith(AppConstants.EmailDomain, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only @ktmtech.com email addresses can be registered.");
            
            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new InvalidOperationException("Email is already registered.");

            if (!await _userRepository.DepartmentExistsAsync(request.DepartmentId))
                throw new InvalidOperationException("Department does not exist.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                DepartmentId = request.DepartmentId,
                DateJoined = request.DateJoined,
                Role = UserRole.Voter // everyone registers as Voter; promote to Admin manually/via separate endpoint
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = _jwtTokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                DepartmentId = user.DepartmentId,
                Token = token
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _jwtTokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                DepartmentId = user.DepartmentId,
                Token = token
            };
        }
        
        public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new KeyNotFoundException("User not found.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Current password is incorrect.");

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.TokenVersion += 1; // invalidates all existing tokens for this user

            await _userRepository.SaveChangesAsync();
        }
        
        public async Task<string?> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            // Don't reveal whether the email exists — always return success-shaped response
            if (user == null) return null;

            var otp = Random.Shared.Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpExpiresAt = DateTime.UtcNow.AddMinutes(10);

            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("Password reset OTP for {Email}: {Otp} (expires in 10 min)", email, otp);

            // Dev-only convenience: return the OTP directly so you can test without an email/SMS provider.
            // Remove this return value once a real mail service (e.g. Mailtrap, SendGrid) is wired up.
            return _env.IsDevelopment() ? otp : null;
        }

        public async Task ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email)
                       ?? throw new InvalidOperationException("Invalid email or OTP.");

            if (user.OtpCode != request.Otp || user.OtpExpiresAt == null || user.OtpExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("Invalid or expired OTP.");

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.OtpCode = null;
            user.OtpExpiresAt = null;
            user.TokenVersion += 1; // invalidate any existing sessions

            await _userRepository.SaveChangesAsync();
        }
    }

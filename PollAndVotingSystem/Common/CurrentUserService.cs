using System.Security.Claims;

namespace PollAndVotingSystem.Common
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public int UserId =>
            int.Parse(User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        public string Role =>
            User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        public int DepartmentId =>
            int.Parse(User?.FindFirstValue("departmentId") ?? "0");

        public bool IsAdmin => Role == "Admin";
    }
}
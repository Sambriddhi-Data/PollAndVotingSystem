using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollAndVotingSystem.Common;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = Policies.RequireAdmin)]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? entityType, [FromQuery] int? entityId)
        {
            return Ok(await _auditLogService.GetLogsAsync(entityType, entityId));
        }
    }
}
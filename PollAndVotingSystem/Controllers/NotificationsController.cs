using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            return Ok(await _notificationService.GetMyNotificationsAsync());
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return NoContent();
        }
    }
}
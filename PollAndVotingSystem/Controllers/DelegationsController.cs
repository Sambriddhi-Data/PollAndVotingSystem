using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollAndVotingSystem.DTOs.Delegation;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DelegationsController : ControllerBase
    {
        private readonly IDelegationService _delegationService;

        public DelegationsController(IDelegationService delegationService)
        {
            _delegationService = delegationService;
        }

        [HttpPost("election/{electionId}")]
        public async Task<IActionResult> Create(int electionId, CreateDelegationRequestDto request)
        {
            return Ok(await _delegationService.CreateAsync(electionId, request));
        }

        [HttpDelete("election/{electionId}")]
        public async Task<IActionResult> Revoke(int electionId)
        {
            await _delegationService.RevokeAsync(electionId);
            return NoContent();
        }

        [HttpGet("election/{electionId}/status")]
        public async Task<IActionResult> GetMyStatus(int electionId)
        {
            return Ok(await _delegationService.GetMyStatusAsync(electionId));
        }
    }

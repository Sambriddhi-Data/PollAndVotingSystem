using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollAndVotingSystem.Common;
using PollAndVotingSystem.DTOs.CandidateApplication;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CandidateApplicationsController : ControllerBase
    {
        private readonly ICandidateApplicationService _service;

        public CandidateApplicationsController(ICandidateApplicationService service)
        {
            _service = service;
        }

        [HttpPost("election/{electionId}/apply")]
        public async Task<IActionResult> Apply(int electionId, ApplyRequestDto request)
        {
            try
            {
                return Ok(await _service.ApplyAsync(electionId, request));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("election/{electionId}")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> GetByElection(int electionId)
        {
            return Ok(await _service.GetByElectionAsync(electionId));
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyApplications()
        {
            return Ok(await _service.GetMyApplicationsAsync());
        }

        [HttpPut("{id}/approve")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                return Ok(await _service.ApproveAsync(id));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id}/reject")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Reject(int id, ReviewRequestDto request)
        {
            try
            {
                return Ok(await _service.RejectAsync(id, request));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpGet("election/{electionId}/candidates")]
        public async Task<IActionResult> GetCandidates(int electionId)
        {
            return Ok(await _service.GetCandidatesByElectionAsync(electionId));
        }
    }
}
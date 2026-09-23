using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollAndVotingSystem.Common;
using PollAndVotingSystem.DTOs.Election;
using PollAndVotingSystem.Services;

namespace PollAndVotingSystem.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ElectionsController : ControllerBase
    {
        private readonly IElectionService _electionService;

        public ElectionsController(IElectionService electionService)
        {
            _electionService = electionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var elections = await _electionService.GetElectionsForCurrentUserAsync();
            return Ok(elections);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(await _electionService.GetByIdAsync(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Create(ElectionRequestDto request)
        {
            try
            {
                var created = await _electionService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Update(int id, ElectionRequestDto request)
        {
            try
            {
                return Ok(await _electionService.UpdateAsync(id, request));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _electionService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("{id}/activate")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                return Ok(await _electionService.ActivateAsync(id));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id}/close")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Close(int id)
        {
            try
            {
                return Ok(await _electionService.CloseAsync(id));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id}/lock-override")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> LockOverride(int id, [FromBody] string reason)
        {
            try
            {
                return Ok(await _electionService.LockOverrideAsync(id, reason));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }
    }

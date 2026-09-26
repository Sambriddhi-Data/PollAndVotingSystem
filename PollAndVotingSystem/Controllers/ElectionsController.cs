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
            return Ok(await _electionService.GetByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Create(ElectionRequestDto request)
        {
                var created = await _electionService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Update(int id, ElectionRequestDto request)
        {
            return Ok(await _electionService.UpdateAsync(id, request));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _electionService.DeleteAsync(id);
            return NoContent();
        }
        
        [HttpPut("{id}/activate")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Activate(int id)
        {
            return Ok(await _electionService.ActivateAsync(id));
        }

        [HttpPut("{id}/close")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> Close(int id)
        {
            return Ok(await _electionService.CloseAsync(id));
            
        }

        [HttpPut("{id}/lock-override")]
        [Authorize(Policy = Policies.RequireAdmin)]
        public async Task<IActionResult> LockOverride(int id, [FromBody] string reason)
        { 
            return Ok(await _electionService.LockOverrideAsync(id, reason)); 
        }
    }

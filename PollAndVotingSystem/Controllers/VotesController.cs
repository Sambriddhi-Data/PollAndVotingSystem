using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollAndVotingSystem.DTOs.Vote;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.Controllers;

[ApiController]
[Route("api/[controller]")] 
[Authorize] 

public class VotesController : ControllerBase
    {
        private readonly IVoteService _voteService;

        public VotesController(IVoteService voteService)
        {
            _voteService = voteService;
        }

        [HttpPost("election/{electionId}")]
        public async Task<IActionResult> CastVote(int electionId, CastVoteRequestDto request)
        {
            await _voteService.CastVoteAsync(electionId, request);
            return Ok(new { message = "Vote cast successfully." });
        }

        [HttpGet("election/{electionId}/status")]
        public async Task<IActionResult> GetMyStatus(int electionId)
        {
            return Ok(await _voteService.GetMyVoteStatusAsync(electionId));
        }

        [HttpGet("election/{electionId}/results")]
        public async Task<IActionResult> GetResults(int electionId)
        {
            return Ok(await _voteService.GetResultsAsync(electionId));
        }
    }

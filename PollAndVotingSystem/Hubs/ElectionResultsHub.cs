using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PollAndVotingSystem.Common;
using PollAndVotingSystem.Repositories;

namespace PollAndVotingSystem.Hubs
{
    [Authorize(Policy = Policies.RequireAdmin)]
    
    public class ElectionResultsHub : Hub
    {
        private readonly IElectionRepository _electionRepository;

        public ElectionResultsHub(IElectionRepository electionRepository)
        {
            _electionRepository = electionRepository;
        }

        // Admin calls this after connecting, to subscribe to one election's live tally.
        public async Task JoinElectionGroup(int electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                throw new HubException("Election not found.");
            }

            var departmentIdClaim = Context.User?.FindFirst("departmentId")?.Value;
            if (departmentIdClaim == null || int.Parse(departmentIdClaim) != election.DepartmentId)
            {
                throw new HubException("You cannot view live results outside your department.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(electionId));
        }

        public async Task LeaveElectionGroup(int electionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(electionId));
        }

        public static string GroupName(int electionId) => $"election-{electionId}";
    }
}
using Microsoft.AspNetCore.SignalR;
using PollAndVotingSystem.Data;
using PollAndVotingSystem.Hubs;
using PollAndVotingSystem.Models;
using PollAndVotingSystem.Repositories;
using PollAndVotingSystem.Services.Interfaces;

namespace PollAndVotingSystem.BackgroundServices
{
    public class ElectionMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ElectionMonitorService> _logger;
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan ApproachingWindow = TimeSpan.FromHours(24);
        private static readonly TimeSpan ClosingSoonWindow = TimeSpan.FromHours(24);

        public ElectionMonitorService(IServiceScopeFactory scopeFactory, ILogger<ElectionMonitorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunCheckAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ElectionMonitorService check failed.");
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task RunCheckAsync()
        {
            using var scope = _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var electionRepository = scope.ServiceProvider.GetRequiredService<IElectionRepository>();
            var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var auditLogService = scope.ServiceProvider.GetRequiredService<IAuditLogService>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ElectionResultsHub>>();

            var now = DateTime.UtcNow;
            var allElections = await electionRepository.GetAllAsync();

            foreach (var election in allElections)
            {
                // --- Approaching: Draft or Active election starting within 24h ---
                if (election.StartDate > now && election.StartDate <= now.Add(ApproachingWindow))
                {
                    var alreadySent = await notificationRepository.HasNotificationTypeBeenSentAsync(
                        election.Id, NotificationType.Approaching);

                    if (!alreadySent)
                    {
                        await notificationService.NotifyDepartmentAsync(
                            election.Id, election.DepartmentId, "Approaching",
                            $"The election \"{election.Title}\" starts soon (on {election.StartDate:yyyy-MM-dd HH:mm} UTC).");
                    }
                }

                // --- Closing soon: Active election ending within 24h ---
                if (election.Status == ElectionStatus.Active &&
                    election.EndDate > now && election.EndDate <= now.Add(ClosingSoonWindow))
                {
                    var alreadySent = await notificationRepository.HasNotificationTypeBeenSentAsync(
                        election.Id, NotificationType.ClosingSoon);

                    if (!alreadySent)
                    {
                        await notificationService.NotifyDepartmentAsync(
                            election.Id, election.DepartmentId, "ClosingSoon",
                            $"The election \"{election.Title}\" closes soon (at {election.EndDate:yyyy-MM-dd HH:mm} UTC). Cast your vote if you haven't already.");
                    }
                }

                // --- Auto-close: Active election past EndDate ---
                if (election.Status == ElectionStatus.Active && election.EndDate <= now)
                {
                    election.Status = ElectionStatus.Closed;
                    electionRepository.Update(election);
                    await electionRepository.SaveChangesAsync();

                    // UserId 0 signals a system-initiated action (no human actor)
                    await auditLogService.LogAsync(
                        0, "ElectionAutoClosed", "Election", election.Id,
                        "Automatically closed by ElectionMonitorService after EndDate elapsed.");

                    await hubContext.Clients
                        .Group(ElectionResultsHub.GroupName(election.Id))
                        .SendAsync("ElectionClosed", new { electionId = election.Id });

                    _logger.LogInformation("Auto-closed Election {ElectionId} after EndDate elapsed.", election.Id);
                }
            }
        }
    }
}
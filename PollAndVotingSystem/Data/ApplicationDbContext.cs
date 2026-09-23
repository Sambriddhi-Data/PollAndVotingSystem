using Microsoft.EntityFrameworkCore;
using PollAndVotingSystem.Models;

namespace PollAndVotingSystem.Data;

public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Election> Elections => Set<Election>();
        public DbSet<CandidateApplication> CandidateApplications => Set<CandidateApplication>();
        public DbSet<Candidate> Candidates => Set<Candidate>();
        public DbSet<Vote> Votes => Set<Vote>();
        public DbSet<Delegation> Delegations => Set<Delegation>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- unique constraints (critical business rules) ----

            // one vote per voter per election
            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.ElectionId, v.VoterId })
                .IsUnique();

            // one delegation per delegator per election
            modelBuilder.Entity<Delegation>()
                .HasIndex(d => new { d.ElectionId, d.DelegatorId })
                .IsUnique();

            // email must be unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ---- prevent cascade-delete cycles (SQL Server will reject multiple cascade paths) ----

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Voter)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.VoterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Candidate)
                .WithMany(c => c.Votes)
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Election)
                .WithMany(e => e.Votes)
                .HasForeignKey(v => v.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Delegation>()
                .HasOne(d => d.Delegator)
                .WithMany(u => u.DelegationsGiven)
                .HasForeignKey(d => d.DelegatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Delegation>()
                .HasOne(d => d.Delegate)
                .WithMany(u => u.DelegationsReceived)
                .HasForeignKey(d => d.DelegateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Delegation>()
                .HasOne(d => d.Election)
                .WithMany(e => e.Delegations)
                .HasForeignKey(d => d.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CandidateApplication>()
                .HasOne(c => c.Election)
                .WithMany(e => e.CandidateApplications)
                .HasForeignKey(c => c.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.Election)
                .WithMany(e => e.Candidates)
                .HasForeignKey(c => c.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // ---- Notification: prevent multiple cascade paths ----

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Election)
                .WithMany()
                .HasForeignKey(n => n.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // ---- CandidateApplication: relationships to User ----

            modelBuilder.Entity<CandidateApplication>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Candidate: relationships to User and Application ----

            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.Application)
                .WithMany()
                .HasForeignKey(c => c.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- User -> Department ----

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Election -> Department ----

            modelBuilder.Entity<Election>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Elections)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        
    }

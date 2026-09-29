using Microsoft.EntityFrameworkCore;

namespace PowerFantasy.ApiService.Data;

public class PowerFantasyDbContext( DbContextOptions<PowerFantasyDbContext> options ) : DbContext( options )
{
    public DbSet<League> Leagues => Set<League>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<WeeklyPoll> WeeklyPolls => Set<WeeklyPoll>();
    public DbSet<PollVote> PollVotes => Set<PollVote>();

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<League>()
            .HasIndex( l => l.SleeperLeagueId )
            .IsUnique();

        modelBuilder.Entity<League>()
            .HasIndex( l => l.PublicSlug )
            .IsUnique();

        modelBuilder.Entity<Team>()
            .HasIndex( t => new { t.LeagueId, t.SleeperRosterId } )
            .IsUnique();

        modelBuilder.Entity<WeeklyPoll>()
            .HasIndex( p => new { p.LeagueId, p.Season, p.Week } )
            .IsUnique();

        modelBuilder.Entity<PollVote>()
            .HasIndex( v => new { v.WeeklyPollId, v.VoterTeamId } )
            .IsUnique();

        modelBuilder.Entity<Team>()
            .HasOne<League>()
            .WithMany()
            .HasForeignKey( t => t.LeagueId )
            .OnDelete( DeleteBehavior.Cascade );

        modelBuilder.Entity<WeeklyPoll>()
            .HasOne<League>()
            .WithMany()
            .HasForeignKey( p => p.LeagueId )
            .OnDelete( DeleteBehavior.Cascade );

        modelBuilder.Entity<PollVote>()
            .HasOne<WeeklyPoll>()
            .WithMany()
            .HasForeignKey( v => v.WeeklyPollId )
            .OnDelete( DeleteBehavior.Cascade );

        modelBuilder.Entity<PollVote>()
            .HasOne<Team>()
            .WithMany()
            .HasForeignKey( v => v.VoterTeamId )
            .OnDelete( DeleteBehavior.Cascade );
    }
}

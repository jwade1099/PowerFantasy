namespace PowerFantasy.ApiService.Data;

public class PollVote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid WeeklyPollId { get; set; }
    public required Guid VoterTeamId { get; set; }

    /// Team ids ordered best-to-worst as submitted by the voter.
    public required List<Guid> RankedTeamIds { get; set; }

    public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;
}

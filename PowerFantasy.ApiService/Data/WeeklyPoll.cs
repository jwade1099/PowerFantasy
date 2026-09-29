namespace PowerFantasy.ApiService.Data;

public class WeeklyPoll
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid LeagueId { get; set; }
    public required string Season { get; set; }
    public required int Week { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

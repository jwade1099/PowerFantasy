namespace PowerFantasy.ApiService.Data;

public class Team
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid LeagueId { get; set; }
    public required int SleeperRosterId { get; set; }
    public required string SleeperOwnerId { get; set; }
    public required string TeamName { get; set; }
    public required string ManagerName { get; set; }
    public string? AvatarUrl { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Ties { get; set; }
    public double PointsFor { get; set; }
    public double PointsAgainst { get; set; }
}

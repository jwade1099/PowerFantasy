namespace PowerFantasy.ApiService.Dashboard;

public record StandingDto(
    Guid TeamId, string TeamName, string ManagerName, string? AvatarUrl,
    int Wins, int Losses, int Ties, double PointsFor, double PointsAgainst );

public record MatchupTeamDto( Guid TeamId, string TeamName, string? AvatarUrl, double Points );

public record MatchupDto( int MatchupId, List<MatchupTeamDto> Teams );

public record DashboardDto( string Season, int Week, List<StandingDto> Standings, List<MatchupDto> Matchups );

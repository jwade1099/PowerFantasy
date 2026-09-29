namespace PowerFantasy.Web;

public record TeamDto( Guid Id, string TeamName, string ManagerName, string? AvatarUrl, int Wins, int Losses, int Ties );

public record PollDto( Guid Id, string Season, int Week, DateTimeOffset CreatedAt, List<TeamDto> Teams, int VotesSubmitted, int TotalVoters );

public record SubmitVoteRequest( Guid VoterTeamId, List<Guid> RankedTeamIds );

public record PollResultDto( Guid TeamId, string TeamName, string ManagerName, string? AvatarUrl, int Wins, int Losses, int Ties, double AverageRank, int VoteCount );

public record PollResultsDto( Guid PollId, string Season, int Week, int VotesSubmitted, int TotalVoters, List<PollResultDto> Rankings );

public record PollSummaryDto( Guid Id, string Season, int Week, DateTimeOffset CreatedAt, int VotesSubmitted, int TotalVoters );

public record StandingDto(
    Guid TeamId, string TeamName, string ManagerName, string? AvatarUrl,
    int Wins, int Losses, int Ties, double PointsFor, double PointsAgainst );

public record MatchupTeamDto( Guid TeamId, string TeamName, string? AvatarUrl, double Points );

public record MatchupDto( int MatchupId, List<MatchupTeamDto> Teams );

public record DashboardDto( string Season, int Week, List<StandingDto> Standings, List<MatchupDto> Matchups );

public record SleeperApiUsage( int CallsThisMinute, int CallsInLookbackWindow, int LookbackMinutes );

public record LeagueDto( Guid Id, string Name, string PublicSlug );

public record CreateLeagueRequest( string SleeperLeagueId, string CommissionerUserId );

public record SleeperLeagueLookupDto( string Name, string Season );

public class PowerFantasyApiClient( HttpClient httpClient )
{
    public async Task<PollDto?> GetCurrentPollAsync( Guid leagueId, CancellationToken ct = default ) =>
        await httpClient.GetFromJsonAsync<PollDto>( $"/api/leagues/{leagueId}/polls/current", ct );

    public async Task<HttpResponseMessage> SubmitVoteAsync( Guid leagueId, Guid pollId, SubmitVoteRequest request, CancellationToken ct = default ) =>
        await httpClient.PostAsJsonAsync( $"/api/leagues/{leagueId}/polls/{pollId}/votes", request, ct );

    public async Task<PollResultsDto?> GetResultsAsync( Guid leagueId, Guid pollId, CancellationToken ct = default ) =>
        await httpClient.GetFromJsonAsync<PollResultsDto>( $"/api/leagues/{leagueId}/polls/{pollId}/results", ct );

    public async Task<List<PollSummaryDto>> GetHistoryAsync( Guid leagueId, CancellationToken ct = default ) =>
        await httpClient.GetFromJsonAsync<List<PollSummaryDto>>( $"/api/leagues/{leagueId}/polls/history", ct ) ?? [];

    public async Task<DashboardDto?> GetDashboardAsync( Guid leagueId, CancellationToken ct = default ) =>
        await httpClient.GetFromJsonAsync<DashboardDto>( $"/api/leagues/{leagueId}/dashboard", ct );

    public async Task<SleeperApiUsage?> GetSleeperUsageAsync( CancellationToken ct = default ) =>
        await httpClient.GetFromJsonAsync<SleeperApiUsage>( "/api/sleeper/usage", ct );

    public async Task<SleeperLeagueLookupDto?> LookupSleeperLeagueAsync( string sleeperLeagueId, CancellationToken ct = default )
    {
        var response = await httpClient.GetAsync( $"/api/sleeper/league/{sleeperLeagueId}", ct );
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<SleeperLeagueLookupDto>( ct ) : null;
    }

    public async Task<HttpResponseMessage> CreateLeagueAsync( CreateLeagueRequest request, CancellationToken ct = default ) =>
        await httpClient.PostAsJsonAsync( "/api/leagues", request, ct );

    public async Task<LeagueDto?> GetMyLeagueAsync( string commissionerUserId, CancellationToken ct = default )
    {
        var response = await httpClient.GetAsync( $"/api/leagues/mine/{commissionerUserId}", ct );
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<LeagueDto>( ct ) : null;
    }

    public async Task<LeagueDto?> GetLeagueBySlugAsync( string slug, CancellationToken ct = default )
    {
        var response = await httpClient.GetAsync( $"/api/leagues/by-slug/{slug}", ct );
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<LeagueDto>( ct ) : null;
    }
}

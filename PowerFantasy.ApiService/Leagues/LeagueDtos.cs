namespace PowerFantasy.ApiService.Leagues;

public record LeagueDto( Guid Id, string Name, string PublicSlug );

public record CreateLeagueRequest( string SleeperLeagueId, string CommissionerUserId );

public enum CreateLeagueResult { Success, SleeperLeagueNotFound, AlreadyClaimed }

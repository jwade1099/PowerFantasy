using System.Text.Json.Serialization;

namespace PowerFantasy.ApiService.Sleeper;

public record SleeperLeague(
    [property: JsonPropertyName( "league_id" )] string LeagueId,
    [property: JsonPropertyName( "name" )] string Name,
    [property: JsonPropertyName( "season" )] string Season,
    [property: JsonPropertyName( "total_rosters" )] int TotalRosters );

public record SleeperRoster(
    [property: JsonPropertyName( "roster_id" )] int RosterId,
    [property: JsonPropertyName( "owner_id" )] string? OwnerId,
    [property: JsonPropertyName( "settings" )] SleeperRosterSettings? Settings );

public record SleeperRosterSettings(
    [property: JsonPropertyName( "wins" )] int Wins,
    [property: JsonPropertyName( "losses" )] int Losses,
    [property: JsonPropertyName( "ties" )] int Ties,
    [property: JsonPropertyName( "fpts" )] int Fpts,
    [property: JsonPropertyName( "fpts_decimal" )] int FptsDecimal,
    [property: JsonPropertyName( "fpts_against" )] int FptsAgainst,
    [property: JsonPropertyName( "fpts_against_decimal" )] int FptsAgainstDecimal );

public record SleeperMatchup(
    [property: JsonPropertyName( "roster_id" )] int RosterId,
    [property: JsonPropertyName( "matchup_id" )] int? MatchupId,
    [property: JsonPropertyName( "points" )] double? Points );

public record SleeperUser(
    [property: JsonPropertyName( "user_id" )] string UserId,
    [property: JsonPropertyName( "display_name" )] string DisplayName,
    [property: JsonPropertyName( "avatar" )] string? Avatar,
    [property: JsonPropertyName( "metadata" )] SleeperUserMetadata? Metadata );

public record SleeperUserMetadata(
    [property: JsonPropertyName( "team_name" )] string? TeamName );

public record SleeperNflState(
    [property: JsonPropertyName( "week" )] int Week,
    [property: JsonPropertyName( "season" )] string Season,
    [property: JsonPropertyName( "season_type" )] string SeasonType );

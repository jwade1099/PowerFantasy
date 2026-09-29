using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace PowerFantasy.ApiService.Sleeper;

public class SleeperClient( HttpClient httpClient, IDistributedCache cache )
{
    private static readonly JsonSerializerOptions JsonOptions = new( JsonSerializerDefaults.Web );

    public Task<SleeperLeague?> GetLeagueAsync( string leagueId, CancellationToken ct = default ) =>
        GetAsync<SleeperLeague>( $"league/{leagueId}", $"sleeper:league:{leagueId}", TimeSpan.FromMinutes( 30 ), ct );

    public async Task<List<SleeperRoster>> GetRostersAsync( string leagueId, CancellationToken ct = default ) =>
        await GetAsync<List<SleeperRoster>>( $"league/{leagueId}/rosters", $"sleeper:rosters:{leagueId}", TimeSpan.FromMinutes( 5 ), ct ) ?? [];

    public async Task<List<SleeperUser>> GetUsersAsync( string leagueId, CancellationToken ct = default ) =>
        await GetAsync<List<SleeperUser>>( $"league/{leagueId}/users", $"sleeper:users:{leagueId}", TimeSpan.FromMinutes( 30 ), ct ) ?? [];

    public Task<SleeperNflState?> GetNflStateAsync( CancellationToken ct = default ) =>
        GetAsync<SleeperNflState>( "state/nfl", "sleeper:state:nfl", TimeSpan.FromMinutes( 15 ), ct );

    public async Task<List<SleeperMatchup>> GetMatchupsAsync( string leagueId, int week, CancellationToken ct = default ) =>
        await GetAsync<List<SleeperMatchup>>( $"league/{leagueId}/matchups/{week}", $"sleeper:matchups:{leagueId}:{week}", TimeSpan.FromMinutes( 2 ), ct ) ?? [];

    /// Sleeper doesn't expose rate-limit headers, so this counts our own outbound (cache-miss) calls
    /// per minute to sanity-check we're well under their documented ~1000 calls/minute guideline.
    public async Task<SleeperApiUsage> GetRecentUsageAsync( int lookbackMinutes = 10, CancellationToken ct = default )
    {
        var now = DateTime.UtcNow;
        var perMinute = new List<int>();

        for (var i = 0; i < lookbackMinutes; i++)
        {
            var value = await cache.GetStringAsync( CallCountKey( now.AddMinutes( -i ) ), ct );
            perMinute.Add( value is null ? 0 : int.Parse( value ) );
        }

        return new SleeperApiUsage( perMinute[0], perMinute.Sum(), lookbackMinutes );
    }

    private async Task<T?> GetAsync<T>( string requestUri, string cacheKey, TimeSpan ttl, CancellationToken ct )
    {
        var cached = await cache.GetStringAsync( cacheKey, ct );
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<T>( cached, JsonOptions );
        }

        await TrackApiCallAsync( ct );

        var response = await httpClient.GetAsync( requestUri, ct );
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync( ct );

        await cache.SetStringAsync( cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        }, ct );

        return JsonSerializer.Deserialize<T>( json, JsonOptions );
    }

    private async Task TrackApiCallAsync( CancellationToken ct )
    {
        var key = CallCountKey( DateTime.UtcNow );
        var current = await cache.GetStringAsync( key, ct );
        var count = (current is null ? 0 : int.Parse( current )) + 1;

        await cache.SetStringAsync( key, count.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes( 15 )
        }, ct );
    }

    private static string CallCountKey( DateTime minute ) => $"sleeper:calls:{minute:yyyyMMddHHmm}";
}

/// CallsThisMinute vs. Sleeper's documented guideline of staying under 1000 calls/minute.
public record SleeperApiUsage( int CallsThisMinute, int CallsInLookbackWindow, int LookbackMinutes );

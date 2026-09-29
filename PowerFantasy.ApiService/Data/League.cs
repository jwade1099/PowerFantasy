namespace PowerFantasy.ApiService.Data;

public class League
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string SleeperLeagueId { get; set; }
    public required string Name { get; set; }

    /// The ASP.NET Identity user id of the commissioner who owns this league (Web project's Identity store).
    public required string CommissionerUserId { get; set; }

    /// Short opaque token used in the public voting link: /{PublicSlug}
    public required string PublicSlug { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

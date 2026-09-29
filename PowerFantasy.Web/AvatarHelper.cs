namespace PowerFantasy.Web;

public static class AvatarHelper
{
    /// Sleeper serves both full-size and thumbnail avatars; use the smaller one for list/table rows.
    public static string? Thumb( string? avatarUrl ) =>
        avatarUrl?.Replace( "/avatars/", "/avatars/thumbs/" );
}

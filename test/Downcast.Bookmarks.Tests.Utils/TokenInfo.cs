namespace Downcast.Bookmarks.Tests.Utils;

public class TokenInfo
{
    public string Token { get; internal set; } = null!;
    public IEnumerable<string> Roles { get; init; } = Enumerable.Empty<string>();
    public string UserId { get; init; } = null!;
    public string Email { get; init; } = null!;
}
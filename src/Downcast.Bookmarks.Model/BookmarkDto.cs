namespace Downcast.Bookmarks.Model;

public class BookmarkDto : BookmarkInputDto
{
    public string Id { get; init; } = null!;
    public string UserId { get; init; } = null!;
    public DateTime Created { get; init; }
}
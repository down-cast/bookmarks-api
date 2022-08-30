namespace Downcast.Bookmarks.Model;

public class BookmarkDto : BookmarkInputDto
{
    public string Id { get; init; } = null!;
    public DateTime Created { get; init; }
}
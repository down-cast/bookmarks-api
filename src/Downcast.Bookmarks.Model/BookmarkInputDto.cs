using System.ComponentModel.DataAnnotations;

namespace Downcast.Bookmarks.Model;

public class BookmarkInputDto
{
    [Required]
    public string ArticleId { get; init; } = null!;
}
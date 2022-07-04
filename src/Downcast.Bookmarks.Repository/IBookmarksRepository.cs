using Downcast.Bookmarks.Model;

namespace Downcast.Bookmarks.Repository;

public interface IBookmarksRepository
{
    Task<string> Create(string userId, string articleId);

    Task<IEnumerable<BookmarkDto>> GetAllByUserId(string userId);

    Task Delete(string userId, string articleId);

    Task DeleteAllByUserId(string userId);

    Task<BookmarkDto> GetByUserIdAndArticleId(string userId, string articleId);
}
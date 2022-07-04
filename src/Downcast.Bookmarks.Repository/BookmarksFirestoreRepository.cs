using Downcast.Bookmarks.Model;
using Downcast.Bookmarks.Repository.Domain;
using Downcast.Bookmarks.Repository.Options;
using Downcast.Common.Errors;

using Google.Cloud.Firestore;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Downcast.Bookmarks.Repository;

public class BookmarksFirestoreRepository : IBookmarksRepository
{
    private readonly CollectionReference _collection;
    private readonly ILogger<BookmarksFirestoreRepository> _logger;

    public BookmarksFirestoreRepository(
        FirestoreDb firestoreDb,
        IOptions<RepositoryOptions> options,
        ILogger<BookmarksFirestoreRepository> logger)
    {
        _logger     = logger;
        _collection = firestoreDb.Collection(options.Value.Collection);
    }

    public async Task<string> Create(string userId, string articleId)
    {
        DocumentReference document = await _collection.AddAsync(new CreateBookmark
        {
            UserId    = userId,
            ArticleId = articleId
        }).ConfigureAwait(false);
        _logger.LogDebug("Created bookmark with {BookmarkId}", document.Id);
        return document.Id;
    }

    public async Task<BookmarkDto> GetByUserIdAndArticleId(string userId, string articleId)
    {
        QuerySnapshot snapshot = await _collection
            .WhereEqualTo(nameof(Bookmark.ArticleId), articleId)
            .WhereEqualTo(nameof(Bookmark.UserId), userId)
            .Limit(1)
            .GetSnapshotAsync()
            .ConfigureAwait(false);
        if (snapshot.Any())
        {
            return CreateBookmarkDto(snapshot[0].ConvertTo<Bookmark>());
        }

        _logger.LogDebug("Bookmark with {ArticleId} and {UserId} was not found", articleId, userId);
        throw new DcException(ErrorCodes.EntityNotFound, "Could not find bookmark");
    }

    public async Task Delete(string userId, string articleId)
    {
        QuerySnapshot snapshotAsync = await _collection
            .WhereEqualTo(nameof(Bookmark.UserId), userId)
            .WhereEqualTo(nameof(Bookmark.ArticleId), articleId)
            .Limit(1)
            .GetSnapshotAsync()
            .ConfigureAwait(false);

        if (snapshotAsync.Any())
        {
            await snapshotAsync[0].Reference.DeleteAsync().ConfigureAwait(false);
            _logger.LogDebug("Delete bookmark with {BookmarkId}", articleId);
        }
    }

    public async Task<IEnumerable<BookmarkDto>> GetAllByUserId(string userId)
    {
        QuerySnapshot snapshots = await GetAllByUserIdInternal(userId).ConfigureAwait(false);
        return snapshots
            .Select(snap => snap.ConvertTo<Bookmark>())
            .Select(CreateBookmarkDto)
            .ToList();
    }

    private static BookmarkDto CreateBookmarkDto(Bookmark bookmark)
    {
        return new BookmarkDto
        {
            Id        = bookmark.Id,
            Created   = bookmark.Created,
            ArticleId = bookmark.ArticleId,
            UserId    = bookmark.UserId
        };
    }

    public async Task DeleteAllByUserId(string userId)
    {
        QuerySnapshot snapshots = await GetAllByUserIdInternal(userId).ConfigureAwait(false);
        foreach (DocumentSnapshot snap in snapshots)
        {
            await snap.Reference.DeleteAsync().ConfigureAwait(false);
        }

        _logger.LogDebug("Deleted {NrOfBookmarks} from {UserId}", snapshots.Count, userId);
    }

    private Task<QuerySnapshot> GetAllByUserIdInternal(string userId)
    {
        return _collection
            .WhereEqualTo(nameof(Bookmark.UserId), userId)
            .GetSnapshotAsync();
    }
}
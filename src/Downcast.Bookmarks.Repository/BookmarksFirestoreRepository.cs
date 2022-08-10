using Downcast.Bookmarks.Model;
using Downcast.Bookmarks.Repository.Domain;
using Downcast.Bookmarks.Repository.Options;
using Downcast.Common.Errors;

using Firestore.Typed.Client;

using Google.Cloud.Firestore;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Downcast.Bookmarks.Repository;

public class BookmarksFirestoreRepository : IBookmarksRepository
{
    private readonly TypedCollectionReference<Bookmark> _collection;
    private readonly ILogger<BookmarksFirestoreRepository> _logger;

    public BookmarksFirestoreRepository(
        FirestoreDb firestoreDb,
        IOptions<RepositoryOptions> options,
        ILogger<BookmarksFirestoreRepository> logger)
    {
        _logger     = logger;
        _collection = firestoreDb.TypedCollection<Bookmark>(options.Value.Collection);
    }

    public async Task<string> Create(string userId, string articleId)
    {
        TypedDocumentReference<Bookmark> document = await _collection.AddAsync(new Bookmark
        {
            UserId    = userId,
            ArticleId = articleId
        }).ConfigureAwait(false);
        _logger.LogDebug("Created bookmark with {BookmarkId}", document.Id);
        return document.Id;
    }

    public async Task<BookmarkDto> GetByUserIdAndArticleId(string userId, string articleId)
    {
        TypedQuerySnapshot<Bookmark> snapshots = await GetAllByUserIdInternal(userId).ConfigureAwait(false);

        if (snapshots.Any())
        {
            return CreateBookmarkDto(snapshots[0].RequiredObject);
        }

        _logger.LogDebug("Bookmark with {ArticleId} and {UserId} was not found", articleId, userId);
        throw new DcException(ErrorCodes.EntityNotFound, "Could not find bookmark");
    }

    public async Task Delete(string userId, string articleId)
    {
        TypedQuerySnapshot<Bookmark> snapshotAsync = await GetBookmarkByUserIdAndArticleId(userId, articleId)
            .ConfigureAwait(false);

        foreach (TypedDocumentSnapshot<Bookmark> snapshot in snapshotAsync)
        {
            await snapshot.Reference.DeleteAsync().ConfigureAwait(false);
            _logger.LogDebug("Delete bookmark with {BookmarkId}", articleId);
        }
    }

    public Task DeleteById(string id)
    {
        return _collection.Document(id).DeleteAsync();
    }

    public async Task<BookmarkDto> GetById(string id)
    {
        TypedDocumentSnapshot<Bookmark> snapshot = await _collection
            .Document(id)
            .GetSnapshotAsync()
            .ConfigureAwait(false);

        if (snapshot.Exists)
        {
            return CreateBookmarkDto(snapshot.RequiredObject);
        }

        throw new DcException(ErrorCodes.EntityNotFound, "Could not find bookmark");
    }

    private Task<TypedQuerySnapshot<Bookmark>> GetBookmarkByUserIdAndArticleId(string userId, string articleId)
    {
        return _collection
            .WhereEqualTo(b => b.UserId, userId)
            .WhereEqualTo(b => b.ArticleId, articleId)
            .Limit(1)
            .GetSnapshotAsync();
    }

    public async Task<IEnumerable<BookmarkDto>> GetAllByUserId(string userId)
    {
        TypedQuerySnapshot<Bookmark> snapshots = await GetAllByUserIdInternal(userId).ConfigureAwait(false);
        return snapshots
            .Select(snap => snap.RequiredObject)
            .Select(CreateBookmarkDto)
            .ToList();
    }

    private static BookmarkDto CreateBookmarkDto(Bookmark bookmark)
    {
        return new BookmarkDto()
        {
            Id        = bookmark.Id,
            Created   = bookmark.Created,
            ArticleId = bookmark.ArticleId,
            UserId    = bookmark.UserId
        };
    }

    public async Task DeleteAllByUserId(string userId)
    {
        TypedQuerySnapshot<Bookmark> snapshots = await GetAllByUserIdInternal(userId).ConfigureAwait(false);
        foreach (TypedDocumentSnapshot<Bookmark> snap in snapshots)
        {
            await snap.Reference.DeleteAsync().ConfigureAwait(false);
        }

        _logger.LogDebug("Deleted {NrOfBookmarks} from {UserId}", snapshots.Count, userId);
    }

    private Task<TypedQuerySnapshot<Bookmark>> GetAllByUserIdInternal(string userId)
    {
        return _collection
            .WhereEqualTo(bookmark => bookmark.UserId, userId)
            .GetSnapshotAsync();
    }
}
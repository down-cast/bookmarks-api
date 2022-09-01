using Downcast.Bookmarks.Model;
using Downcast.Bookmarks.Repository.Domain;
using Downcast.Bookmarks.Repository.Options;
using Downcast.Common.Errors;

using Firestore.Typed.Client;
using Firestore.Typed.Client.Extensions;

using Google.Cloud.Firestore;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Downcast.Bookmarks.Repository;

public class BookmarksFirestoreRepository : IBookmarksRepository
{
    private readonly FirestoreDb _firestoreDb;
    private readonly IOptions<RepositoryOptions> _options;
    private readonly ILogger<BookmarksFirestoreRepository> _logger;


    public BookmarksFirestoreRepository(
        FirestoreDb firestoreDb,
        IOptions<RepositoryOptions> options,
        ILogger<BookmarksFirestoreRepository> logger)
    {
        _firestoreDb = firestoreDb;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Returns a sub collection of bookmarks for the given userId
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private TypedCollectionReference<Bookmark> GetCollection(string userId)
    {
        return _firestoreDb
            .Collection(_options.Value.UsersCollectionName)
            .Document(userId)
            .TypedCollection<Bookmark>(_options.Value.BookmarksCollectionName);
    }


    /// <summary>
    /// Adds a new bookmark to the given user's bookmarks collection
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="articleId"></param>
    /// <returns></returns>
    public Task<string> Create(string userId, string articleId)
    {
        return _firestoreDb.RunTypedTransactionAsync<Bookmark, string>(async transaction =>
        {
            TypedDocumentSnapshot<Bookmark> docSnapshot = await transaction
                .GetSnapshotAsync(GetBookmarkDocument(userId, articleId))
                .ConfigureAwait(false);

            if (docSnapshot.Exists)
            {
                return docSnapshot.Id;
            }

            transaction.Create(docSnapshot.Reference, new Bookmark
            {
                ArticleId = articleId,
                Created = DateTime.UtcNow
            });

            _logger.LogDebug("Created bookmark with {BookmarkId}", docSnapshot.Id);
            return docSnapshot.Id;
        });
    }


    /// <summary>
    /// Get a bookmark for the given userId and articleId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="articleId"></param>
    /// <returns></returns>
    /// <exception cref="DcException"></exception>
    public async Task<BookmarkDto> GetByUserIdAndArticleId(string userId, string articleId)
    {
        TypedDocumentSnapshot<Bookmark> bookmark = await GetBookmarkSnapshot(userId, articleId).ConfigureAwait(false);
        if (bookmark.Exists)
        {
            return CreateBookmarkDto(bookmark.RequiredObject);
        }

        _logger.LogDebug("Bookmark with {ArticleId} and {UserId} was not found", articleId, userId);
        throw new DcException(ErrorCodes.EntityNotFound, "Could not find bookmark");
    }

    private Task<TypedDocumentSnapshot<Bookmark>> GetBookmarkSnapshot(string userId, string articleId)
    {
        return GetBookmarkDocument(userId, articleId).GetSnapshotAsync();
    }

    private TypedDocumentReference<Bookmark> GetBookmarkDocument(string userId, string articleId)
    {
        return GetCollection(userId).Document(articleId);
    }

    /// <summary>
    /// Delete a bookmark for the given userId and articleId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="articleId"></param>
    public async Task Delete(string userId, string articleId)
    {
        await GetBookmarkDocument(userId, articleId).DeleteAsync().ConfigureAwait(false);
        _logger.LogDebug("Deleted bookmark for {UserId} and {ArticleId}", userId, articleId);
    }

    /// <summary>
    /// Gets all bookmarks for the given userId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<BookmarkDto> GetBookmarks(string userId, BookmarksFilter filter)
    {
        IAsyncEnumerable<TypedDocumentSnapshot<Bookmark>> bookmarksStream = GetCollection(userId)
            .OrderByDescending(bookmark => bookmark.Created)
            .Offset(filter.Skip)
            .Limit(filter.Top)
            .StreamAsync();

        await foreach (TypedDocumentSnapshot<Bookmark> snapshot in bookmarksStream.ConfigureAwait(false))
        {
            yield return CreateBookmarkDto(snapshot.RequiredObject);
        }
    }

    private static BookmarkDto CreateBookmarkDto(Bookmark bookmark) => new()
    {
        Created = bookmark.Created,
        ArticleId = bookmark.ArticleId
    };

    /// <summary>
    /// Deletes all bookmarks for the given userId
    /// </summary>
    /// <param name="userId"></param>
    public async Task DeleteAll(string userId)
    {
        TypedQuerySnapshot<Bookmark> snapshots = await GetCollection(userId).GetSnapshotAsync().ConfigureAwait(false);
        foreach (TypedDocumentSnapshot<Bookmark> snap in snapshots)
        {
            await snap.Reference.DeleteAsync().ConfigureAwait(false);
        }

        _logger.LogDebug("Deleted {NrOfBookmarks} from {UserId}", snapshots.Count, userId);
    }
}
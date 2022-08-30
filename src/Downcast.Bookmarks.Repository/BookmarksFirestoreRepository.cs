using System.Runtime.CompilerServices;

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
    /// returns the sub-collection of bookmarks for the given user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private TypedCollectionReference<Bookmark> GetCollection(string userId) => _firestoreDb
        .Collection(_options.Value.UsersCollectionName)
        .Document(userId)
        .TypedCollection<Bookmark>(_options.Value.BookmarksCollectionName);

    public async Task<string> Create(string userId, string articleId)
    {
        TypedDocumentReference<Bookmark> document = await GetCollection(userId).AddAsync(new Bookmark
        {
            UserId = userId,
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
        TypedQuerySnapshot<Bookmark> snapshot = await GetBookmarkByUserIdAndArticleId(userId, articleId)
            .ConfigureAwait(false);

        foreach (TypedDocumentSnapshot<Bookmark> documentSnapshot in snapshot)
        {
            await documentSnapshot.Reference.DeleteAsync().ConfigureAwait(false);
            _logger.LogDebug("Deleted bookmark with {ArticleId} and {Id}", articleId, documentSnapshot.Id);
        }
    }

    public Task DeleteById(string userId, string bookmarkId)
    {
        return GetCollection(userId).Document(bookmarkId).DeleteAsync();
    }

    public async Task<BookmarkDto> GetById(string userId, string bookmarkId)
    {
        TypedDocumentSnapshot<Bookmark> snapshot = await GetCollection(userId)
            .Document(bookmarkId)
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
        return GetCollection(userId)
            .WhereEqualTo(b => b.ArticleId, articleId)
            .Limit(1)
            .GetSnapshotAsync();
    }

    public async IAsyncEnumerable<BookmarkDto> GetBookmarksByUserId(string userId, BookmarksFilter filter)
    {
        IAsyncEnumerable<TypedDocumentSnapshot<Bookmark>> bookmarksStream = GetCollection(userId)
            .Offset(filter.Skip)
            .Limit(filter.Top)
            .StreamAsync();
        
        await foreach (TypedDocumentSnapshot<Bookmark> snapshot in bookmarksStream.ConfigureAwait(false))
        {
            yield return CreateBookmarkDto(snapshot.RequiredObject);
        }
    }

    private static BookmarkDto CreateBookmarkDto(Bookmark bookmark)
    {
        return new BookmarkDto()
        {
            Id = bookmark.Id,
            Created = bookmark.Created,
            ArticleId = bookmark.ArticleId
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
        return GetCollection(userId).GetSnapshotAsync();
    }
}
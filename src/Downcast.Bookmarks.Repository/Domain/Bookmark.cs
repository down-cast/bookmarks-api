using Google.Cloud.Firestore;

namespace Downcast.Bookmarks.Repository.Domain;

[FirestoreData]
internal class Bookmark
{
    [FirestoreDocumentId]
    public string ArticleId { get; init; } = null!;

    [FirestoreProperty]
    public DateTime Created { get; init; }
}
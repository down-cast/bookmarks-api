using Google.Cloud.Firestore;

namespace Downcast.Bookmarks.Repository.Domain;

[FirestoreData]
internal class Bookmark
{
    [FirestoreDocumentId]
    public string Id { get; set; } = null!;

    [FirestoreProperty]
    public string ArticleId { get; init; } = null!;

    [FirestoreProperty]
    public string UserId { get; init; } = null!;

    [FirestoreDocumentCreateTimestamp]
    public DateTime Created { get; set; }
}
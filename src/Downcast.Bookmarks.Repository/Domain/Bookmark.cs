using Google.Cloud.Firestore;

namespace Downcast.Bookmarks.Repository.Domain;

[FirestoreData]
internal class Bookmark
{
    [FirestoreDocumentId]
    public string ArticleId { get; init; } = null!;

    [FirestoreDocumentCreateTimestamp]
    public DateTime Created { get; set; }
}
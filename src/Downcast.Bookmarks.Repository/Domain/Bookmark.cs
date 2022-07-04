using Google.Cloud.Firestore;

namespace Downcast.Bookmarks.Repository.Domain;

[FirestoreData]
internal class Bookmark : CreateBookmark
{
    [FirestoreDocumentId]
    public string Id { get; set; } = null!;
    [FirestoreDocumentCreateTimestamp]
    public DateTime Created { get; set; }
}
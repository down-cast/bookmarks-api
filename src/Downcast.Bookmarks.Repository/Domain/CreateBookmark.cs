using Google.Cloud.Firestore;

namespace Downcast.Bookmarks.Repository.Domain;

[FirestoreData]
internal class CreateBookmark
{
    [FirestoreProperty]
    public string ArticleId { get; init; } = null!;

    [FirestoreProperty]
    public string UserId { get; init; } = null!;


}
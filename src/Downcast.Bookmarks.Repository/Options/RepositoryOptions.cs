using System.ComponentModel.DataAnnotations;

namespace Downcast.Bookmarks.Repository.Options;

public class RepositoryOptions
{
    public const string SectionName = "RepositoryOptions";
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "Collection name cannot be null or empty")]
    public string Collection { get; set; } = null!;
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "ProjectId is required")]
    public string ProjectId { get; set; } = null!;
}
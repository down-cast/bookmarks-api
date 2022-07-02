using Microsoft.AspNetCore.Mvc;

namespace Downcast.Bookmarks.API.Controllers;

[ApiController]
[Route("api/v1/bookmarks")]
public class BookmarksController : ControllerBase
{
    private readonly ILogger<BookmarksController> _logger;

    public BookmarksController(ILogger<BookmarksController> logger)
    {
        _logger = logger;
    }
}
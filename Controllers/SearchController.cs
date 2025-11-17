using Microsoft.AspNetCore.Mvc;
using Vdoflix.Web.Services;

namespace Vdoflix.Web.Controllers;

public class SearchController : Controller
{
    private readonly TmdbService _tmdb;
    public SearchController(TmdbService tmdb) => _tmdb = tmdb;

    [HttpGet]
    public async Task<IActionResult> Index(string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return View(model: null);
        var results = await _tmdb.SearchAsync(q);
        return View(results);
    }
}

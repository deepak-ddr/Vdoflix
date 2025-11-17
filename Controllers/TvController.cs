using Microsoft.AspNetCore.Mvc;
using Vdoflix.Web.Services;

namespace Vdoflix.Web.Controllers;

public class TvController : Controller
{
    private readonly TmdbService _tmdb;
    public TvController(TmdbService tmdb) => _tmdb = tmdb;

    [HttpGet("tv/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var tv = await _tmdb.GetTVAsync(id);
        return View(tv);
    }
}

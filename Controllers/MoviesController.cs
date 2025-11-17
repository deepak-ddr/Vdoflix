using Microsoft.AspNetCore.Mvc;
using Vdoflix.Web.Services;

namespace Vdoflix.Web.Controllers;

public class MoviesController : Controller
{
    private readonly TmdbService _tmdb;
    public MoviesController(TmdbService tmdb) => _tmdb = tmdb;

    [HttpGet("movies/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var movie = await _tmdb.GetMovieAsync(id);
        return View(movie);
    }
}

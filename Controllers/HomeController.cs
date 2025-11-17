using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Vdoflix.Web.Models;

namespace Vdoflix.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Vdoflix.Web.Data.ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, Vdoflix.Web.Data.ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index()
    {
        var sections = GetSampleSections();
        var featured = sections.FirstOrDefault()?.Items.FirstOrDefault();
        var vm = new HomeViewModel
        {
            Featured = featured,
            Sections = sections
        };

        if (User?.Identity?.IsAuthenticated == true)
        {
            var userId = User?.Identity?.Name ?? string.Empty;
            var cw = _db.WatchProgress
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.UpdatedAt)
                .Take(10)
                .ToList();
            vm.ContinueWatching = cw;
        }

        return View(vm);
    }

    private static List<Section> GetSampleSections()
    {
        // Using royalty-free sample images from TMDB via image placeholders or generic picsum as demo
        // In real app, integrate a data source/API. Here we just seed a few examples.
        List<MovieItem> Make(string name, int count, int seed) =>
            Enumerable.Range(1, count).Select(i => new MovieItem
            {
                Title = $"{name} {i}",
                ImageUrl = $"https://image.tmdb.org/t/p/w300/8YFL5QQVPy3AgrEQxNYVSgiPEbe.jpg?{seed+i}",
                BackdropUrl = $"https://image.tmdb.org/t/p/w1280/2RSirqZG949GuRwN38MYCIGG4Od.jpg?{seed+i}",
                Description = "A gripping tale set in a world of intrigue. This is placeholder copy for the demo.",
                Rating = Math.Round(6 + 4 * Math.Abs(Math.Sin((seed + i) * 1.7)), 1)
            }).ToList();

        return new List<Section>
        {
            new Section { Title = "Trending Now", Items = Make("Trending", 12, 1) },
            new Section { Title = "Top Picks", Items = Make("Top Pick", 10, 2) },
            new Section { Title = "New Releases", Items = Make("New", 14, 3) },
            new Section { Title = "Because you watched Sci‑Fi", Items = Make("Sci‑Fi", 16, 4) },
        };
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

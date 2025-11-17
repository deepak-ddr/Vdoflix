using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vdoflix.Web.Data;
using Vdoflix.Web.Models;

namespace Vdoflix.Web.Controllers;

[Authorize]
public class WatchController : Controller
{
    private readonly ApplicationDbContext _db;
    public WatchController(ApplicationDbContext db) => _db = db;

    [HttpGet("watch/{id}")]
    [AllowAnonymous]
    public IActionResult Index(string id, string? title = null, string? poster = null)
    {
        ViewData["Title"] = title ?? "Watch";
        ViewData["Poster"] = poster;
        ViewData["Id"] = id;
        return View();
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Progress([FromBody] ProgressDto dto)
    {
        var userId = User?.Identity?.Name ?? User.FindFirst("sub")?.Value ?? User.FindFirst("uid")?.Value ?? string.Empty;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var entity = await _db.WatchProgress.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == dto.MovieId);
        if (entity is null)
        {
            entity = new WatchProgress
            {
                UserId = userId,
                MovieId = dto.MovieId,
                Title = dto.Title ?? string.Empty,
                PosterUrl = dto.PosterUrl,
                DurationSeconds = dto.DurationSeconds,
                PositionSeconds = dto.PositionSeconds,
                UpdatedAt = DateTime.UtcNow
            };
            _db.WatchProgress.Add(entity);
        }
        else
        {
            entity.PositionSeconds = dto.PositionSeconds;
            entity.DurationSeconds = dto.DurationSeconds;
            entity.Title = dto.Title ?? entity.Title;
            entity.PosterUrl = dto.PosterUrl ?? entity.PosterUrl;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();
        return Ok();
    }

    public class ProgressDto
    {
        public string MovieId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? PosterUrl { get; set; }
        public double PositionSeconds { get; set; }
        public double DurationSeconds { get; set; }
    }
}

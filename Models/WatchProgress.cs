namespace Vdoflix.Web.Models;

public class WatchProgress
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string MovieId { get; set; } = string.Empty; // TMDB id or internal id
    public string Title { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public double PositionSeconds { get; set; }
    public double DurationSeconds { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

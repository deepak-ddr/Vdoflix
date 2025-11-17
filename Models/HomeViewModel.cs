namespace Vdoflix.Web.Models;

public class HomeViewModel
{
    public MovieItem? Featured { get; set; }
    public List<Section> Sections { get; set; } = new();
    public List<WatchProgress> ContinueWatching { get; set; } = new();
}

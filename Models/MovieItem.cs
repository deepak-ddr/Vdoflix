namespace Vdoflix.Web.Models;

public class MovieItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty; // poster thumbnail
    public string BackdropUrl { get; set; } = string.Empty; // wide hero backdrop
    public string Description { get; set; } = string.Empty;
    public double Rating { get; set; }
}

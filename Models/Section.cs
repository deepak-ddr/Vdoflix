namespace Vdoflix.Web.Models;

public class Section
{
    public string Title { get; set; } = string.Empty;
    public List<MovieItem> Items { get; set; } = new();
}

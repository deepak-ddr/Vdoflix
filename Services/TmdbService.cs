using System.Net.Http.Headers;
using System.Text.Json;

namespace Vdoflix.Web.Services;

public class TmdbService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public TmdbService(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("tmdb");
        var apiKey = _config["Tmdb:ApiKey"] ?? string.Empty;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        return client;
    }

    public async Task<object?> SearchAsync(string query, int page = 1, CancellationToken ct = default)
    {
        var client = CreateClient();
        var res = await client.GetAsync($"search/multi?query={Uri.EscapeDataString(query)}&page={page}", ct);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<object>(json, _json);
    }

    public async Task<object?> GetMovieAsync(int id, CancellationToken ct = default)
    {
        var client = CreateClient();
        var res = await client.GetAsync($"movie/{id}?append_to_response=videos,images,credits,recommendations", ct);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<object>(json, _json);
    }

    public async Task<object?> GetTVAsync(int id, CancellationToken ct = default)
    {
        var client = CreateClient();
        var res = await client.GetAsync($"tv/{id}?append_to_response=videos,images,credits,recommendations", ct);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<object>(json, _json);
    }
}

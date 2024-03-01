using System.Text.Json;
using SearchingAggregator.Models;

namespace SearchingAggregator.Services;

internal class GoogleSearchService(IConfiguration configuration) : ISearchService {
    private string? _apiKey;
    private string? _searchEngineId;

    public async Task<SearchResponse> GetSearchResponse(string query) {
        _apiKey = configuration["GoogleCustomSearch:ApiKey"];
        _searchEngineId = configuration["GoogleCustomSearch:SearchEngineId"];

        var httpClient = new HttpClient();
        var url = $"https://www.googleapis.com/customsearch/v1?key={_apiKey}&cx={_searchEngineId}&q={query}";

        HttpResponseMessage response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode) {
            return new SearchResponse();
        }

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        SearchResponse searchResults = JsonSerializer.Deserialize<SearchResponse>(content, options) ?? new SearchResponse();

        return searchResults;
    }
}
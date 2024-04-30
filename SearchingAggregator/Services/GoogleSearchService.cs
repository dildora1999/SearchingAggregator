using System.Text.Json;
using SearchingAggregator.Database.Repositories;
using SearchingAggregator.Serialization;

namespace SearchingAggregator.Services;

internal class GoogleSearchService(IConfiguration configuration, ISearchResultsRepository searchResultsRepository) : ISearchService {
    private string? _apiKey;
    private string? _searchEngineId;

    public async Task<SearchResults> GetSearchResponse(string query) {
        _apiKey = configuration["GoogleCustomSearch:ApiKey"];
        _searchEngineId = configuration["GoogleCustomSearch:SearchEngineId"];

        //try to use DI
        var httpClient = new HttpClient();
        var url = $"https://www.googleapis.com/customsearch/v1?key={_apiKey}&cx={_searchEngineId}&q={query}";

        HttpResponseMessage response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode) {
            return new SearchResults();
        }

        var searchResults = await DeserializeResponse(response);
        
        //insert into database
        await searchResultsRepository.InsertQueryResults(query, searchResults);

        return searchResults;
    }

    private async Task<SearchResults> DeserializeResponse(HttpResponseMessage response) {
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<SearchResults>(content, options) ?? new SearchResults();
    }
}
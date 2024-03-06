using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SearchingAggregator.Database;
using SearchingAggregator.Models;

namespace SearchingAggregator.Services;

internal class GoogleSearchService(IConfiguration configuration) : ISearchService {
    private string? _apiKey;
    private string? _searchEngineId;

    public async Task<SearchResults> GetSearchResponse(string query) {
        _apiKey = configuration["GoogleCustomSearch:ApiKey"];
        _searchEngineId = configuration["GoogleCustomSearch:SearchEngineId"];

        var httpClient = new HttpClient();
        var url = $"https://www.googleapis.com/customsearch/v1?key={_apiKey}&cx={_searchEngineId}&q={query}";

        HttpResponseMessage response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode) {
            return new SearchResults();
        }

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        SearchResults searchResults = JsonSerializer.Deserialize<SearchResults>(content, options) ?? new SearchResults();
        await using (var dbContext = new SearchResultsDbContext(new DbContextOptions<SearchResultsDbContext>())) {
            var searchResultsEntity = new SearchResultsEntity { Query = query, CreationDate = DateTime.Now, SearchResultItemEntities = new List<SearchResultItemEntity>() };

            foreach (SearchResultItem item in searchResults.Items) {
                searchResultsEntity.SearchResultItemEntities.Add(new SearchResultItemEntity {
                    Query = query, Title = item.Title, Link = item.Link, Description = item.Snippet
                });
            }

            dbContext.SearchResultsEntities.Add(searchResultsEntity);
        }

        return searchResults;
    }
}
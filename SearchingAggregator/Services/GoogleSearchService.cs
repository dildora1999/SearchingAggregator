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
        SearchResults searchResults = new SearchResults();
        
        await using var dbContext = new SearchResultsDbContext(new DbContextOptions<SearchResultsDbContext>());
        await dbContext.Database.EnsureCreatedAsync();
        
        var searchResultsFromDb = dbContext.SearchResultItems
            .Include(p => p.SearchResultsEntity).Where(s => s.SearchResultsEntity.Query.Equals(query));
        if (searchResultsFromDb.Any()) {
            foreach (SearchResultItemEntity item in searchResultsFromDb) {
                searchResults.Items.Add(new SearchResultItem {
                    Title = item.Title, Link = item.Link, Snippet = item.Description,
                });
            }

            return searchResults;
        }
            

        var httpClient = new HttpClient();
        var url = $"https://www.googleapis.com/customsearch/v1?key={_apiKey}&cx={_searchEngineId}&q={query}";

        HttpResponseMessage response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode) {
            return new SearchResults();
        }

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        searchResults = JsonSerializer.Deserialize<SearchResults>(content, options) ?? new SearchResults();
        var searchResultsEntity = new SearchResultsEntity { Query = query, CreationDate = DateTime.Now, SearchResultItemEntities = new List<SearchResultItemEntity>() };
        dbContext.SearchResultsEntities.Add(searchResultsEntity);
        foreach (SearchResultItem item in searchResults.Items) {
            dbContext.SearchResultItems.Add(new SearchResultItemEntity {
                Title = item.Title, Link = item.Link, Description = item.Snippet,
                SearchResultsEntity = searchResultsEntity
            });
        }
        
        await dbContext.SaveChangesAsync();
        return searchResults;
    }
}
using Microsoft.EntityFrameworkCore;
using SearchingAggregator.Database.Entities;
using SearchingAggregator.Serialization;

namespace SearchingAggregator.Database.Repositories;

internal class SearchResultsRepository(SearchResultsDbContext searchResultsDbContext) : ISearchResultsRepository {
    public async Task<SearchResults?> FindResultsByQuery(string query) {
        SearchResults results = new SearchResults();
        await searchResultsDbContext.Database.EnsureCreatedAsync();
        var searchResultsFromDb = searchResultsDbContext.SearchResultItems
            .Include(p => p.SearchResultsEntity).Where(s => s.SearchResultsEntity.Query.Equals(query));
        if (searchResultsFromDb.Any()) {
            foreach (SearchResultItemEntity item in searchResultsFromDb) {
                results.Items.Add(new SearchResultItem {
                    Title = item.Title, Link = item.Link, Snippet = item.Description,
                });
            }

            return results;
        }

        return null;
    }

    public async Task InsertQueryResults(string query, SearchResults results) {
        var searchResultsEntity = new SearchResultsEntity { Query = query, CreationDate = DateTime.Now, SearchResultItemEntities = new List<SearchResultItemEntity>() };
        searchResultsDbContext.SearchResultsEntities.Add(searchResultsEntity);
        foreach (SearchResultItem item in results.Items) {
            searchResultsDbContext.SearchResultItems.Add(new SearchResultItemEntity {
                Title = item.Title, Link = item.Link, Description = item.Snippet,
                SearchResultsEntity = searchResultsEntity
            });
        }
        
        await searchResultsDbContext.SaveChangesAsync();
    }
}
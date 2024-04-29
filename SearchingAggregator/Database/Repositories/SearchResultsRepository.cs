using Microsoft.EntityFrameworkCore;
using SearchingAggregator.Database.Entities;
using SearchingAggregator.Serialization;

namespace SearchingAggregator.Database.Repositories;

internal class SearchResultsRepository : ISearchResultsRepository {
    public async Task<SearchResults?> FindResultsByQuery(string query) {
        SearchResults results = new SearchResults();
        //try to use DI
        await using var dbContext = new SearchResultsDbContext(new DbContextOptions<SearchResultsDbContext>());
        await dbContext.Database.EnsureCreatedAsync();
        var searchResultsFromDb = dbContext.SearchResultItems
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
        //try to use DI
        await using var dbContext = new SearchResultsDbContext(new DbContextOptions<SearchResultsDbContext>());
        var searchResultsEntity = new SearchResultsEntity { Query = query, CreationDate = DateTime.Now, SearchResultItemEntities = new List<SearchResultItemEntity>() };
        dbContext.SearchResultsEntities.Add(searchResultsEntity);
        foreach (SearchResultItem item in results.Items) {
            dbContext.SearchResultItems.Add(new SearchResultItemEntity {
                Title = item.Title, Link = item.Link, Description = item.Snippet,
                SearchResultsEntity = searchResultsEntity
            });
        }
        
        await dbContext.SaveChangesAsync();
    }
}
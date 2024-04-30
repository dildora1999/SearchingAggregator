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
        var resultFromDb = await searchResultsDbContext.SearchResultsEntities.FindAsync(query);
        if (resultFromDb is not null) {
            searchResultsDbContext.SearchResultsEntities.Update(resultFromDb);
            resultFromDb.CreationDate = DateTime.Now;
            
            foreach (SearchResultItem item in results.Items) {
                var itemFromDb = await searchResultsDbContext.SearchResultItems.FindAsync(item.Title);
                if (itemFromDb is not null) {
                    searchResultsDbContext.SearchResultItems.Update(itemFromDb);
                    itemFromDb.Link = item.Link;
                    itemFromDb.Description = item.Snippet;
                    itemFromDb.SearchResultsEntity = resultFromDb;
                }
            }
        }
        else {
            var searchResultsEntity = new SearchResultsEntity { Query = query, CreationDate = DateTime.Now, SearchResultItemEntities = new List<SearchResultItemEntity>() };
            await searchResultsDbContext.SearchResultsEntities.AddAsync(searchResultsEntity);
            foreach (SearchResultItem item in results.Items) {
                await searchResultsDbContext.SearchResultItems.AddAsync(new SearchResultItemEntity {
                    Title = item.Title, Link = item.Link, Description = item.Snippet,
                    SearchResultsEntity = searchResultsEntity
                });
            }
        }
        
        await searchResultsDbContext.SaveChangesAsync();
    }
}
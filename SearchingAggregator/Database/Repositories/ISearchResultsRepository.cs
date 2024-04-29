using SearchingAggregator.Serialization;

namespace SearchingAggregator.Database.Repositories;

public interface ISearchResultsRepository {
    Task<SearchResults?> FindResultsByQuery(string query);
    Task InsertQueryResults(string query, SearchResults results);
}
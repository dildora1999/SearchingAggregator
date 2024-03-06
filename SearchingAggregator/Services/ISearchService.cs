using SearchingAggregator.Models;

namespace SearchingAggregator.Services;

public interface ISearchService {
    Task<SearchResults> GetSearchResponse(string query);
}
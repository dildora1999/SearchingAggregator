using SearchingAggregator.Serialization;

namespace SearchingAggregator.Services;

public interface ISearchService {
    Task<SearchResults> GetSearchResponse(string query);
}
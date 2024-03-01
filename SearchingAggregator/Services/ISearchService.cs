using SearchingAggregator.Models;

namespace SearchingAggregator.Services;

public interface ISearchService {
    Task<SearchResponse> GetSearchResponse(string query);
}
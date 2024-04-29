using Microsoft.AspNetCore.Mvc;
using SearchingAggregator.Database.Repositories;
using SearchingAggregator.Serialization;
using SearchingAggregator.Services;

namespace SearchingAggregator.Controllers;

public class HomeController(ISearchService searchService, ISearchResultsRepository searchResultsRepository) : Controller {
    public IActionResult Index() {
        return View();
    }
    
    public IActionResult Database() {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query) {
        if (string.IsNullOrEmpty(query)) {
            return View("Error");
        }

        SearchResults searchResults = await searchService.GetSearchResponse(query);
        ViewBag.Query = query;

        return View("Index", searchResults);
    }
    
    [HttpGet]
    public async Task<IActionResult> FindFromDatabase(string query) {
        if (string.IsNullOrEmpty(query)) {
            return View("Error");
        }

        SearchResults? searchResults = await searchResultsRepository.FindResultsByQuery(query);
        ViewBag.Query = query;
        return View("Index", searchResults);
    }
}
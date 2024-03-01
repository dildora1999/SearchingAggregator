using Microsoft.AspNetCore.Mvc;
using SearchingAggregator.Models;
using SearchingAggregator.Services;

namespace SearchingAggregator.Controllers;

public class HomeController(ISearchService searchService) : Controller {
    public IActionResult Index() {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query) {
        if (string.IsNullOrEmpty(query)) {
            return View("Error");
        }

        SearchResponse searchResponse = await searchService.GetSearchResponse(query);
        ViewBag.Query = query;

        return View("Index", searchResponse);
    }
}
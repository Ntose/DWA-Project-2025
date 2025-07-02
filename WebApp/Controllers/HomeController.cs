using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApp.ViewModels;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _http;
    public HomeController(IHttpClientFactory http) => _http = http;

    public async Task<IActionResult> Index()
    {
        var client = _http.CreateClient("ApiClient");
        // fetch first 3 items
        var resp = await client.GetAsync("/api/heritage/search?page=1&count=3");
        if (!resp.IsSuccessStatusCode)
            return View(new List<HeritageListItemVm>());

        var json = await resp.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<List<HeritageListItemVm>>(json,
                      new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(items);
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;
using WebApp.ViewModels;

[Authorize]
public class ProfileController : Controller
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly AuthService _authService;

    public ProfileController(IHttpClientFactory httpFactory, AuthService authService)
    {
        _httpFactory = httpFactory;
        _authService = authService;
    }

    // GET: /Profile
    public async Task<IActionResult> Index()
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var resp = await client.GetAsync("/api/profile");
        if (!resp.IsSuccessStatusCode)
        {
            ViewBag.Error = "Unable to load profile.";
            return View(new UserProfileVm());
        }

        var json = await resp.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<UserProfileVm>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(model);
    }

    // GET: /Profile/Settings
    public async Task<IActionResult> Settings()
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var resp = await client.GetAsync("/api/profile");
        var json = await resp.Content.ReadAsStringAsync();
        var model = JsonSerializer.Deserialize<UserProfileVm>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(model);
    }

    // POST: /Profile/Settings
    [HttpPost]
    public async Task<IActionResult> Settings(UserProfileVm model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var json = JsonSerializer.Serialize(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var resp = await client.PutAsync("/api/profile", content);
        if (resp.IsSuccessStatusCode)
        {
            TempData["Success"] = "Profile updated.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = "Failed to update profile.";
        return View(model);
    }

    // GET: /Profile/Favorites
    public async Task<IActionResult> Favorites()
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var resp = await client.GetAsync("/api/profile/favorites");
        if (!resp.IsSuccessStatusCode)
        {
            ViewBag.Error = "Unable to load favorites.";
            return View(new List<HeritageListItemVm>());
        }

        var json = await resp.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<HeritageListItemVm>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(data);
    }
}

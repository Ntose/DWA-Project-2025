using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WebAPI.Data.Entities;
using WebApp.ViewModels;

public class TopicController : Controller
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly AuthService _authService;

    public TopicController(IHttpClientFactory httpFactory, AuthService authService)
    {
        _httpFactory = httpFactory;
        _authService = authService;
    }

    // Public list: /Topic
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var resp = await client.GetAsync("/api/topics");
        if (!resp.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not load topics.";
            return View(new List<TopicVm>());
        }

        var json = await resp.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<TopicVm>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(data);
    }

    // Admin management list: /Topic/Manage
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var resp = await client.GetAsync("/api/topics");
        var json = await resp.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<TopicVm>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(data);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new TopicVm());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(TopicVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var json = JsonSerializer.Serialize(vm);
        var resp = await client.PostAsync("/api/topics",
            new StringContent(json, Encoding.UTF8, "application/json"));

        if (resp.IsSuccessStatusCode)
            return RedirectToAction(nameof(Manage));

        ModelState.AddModelError("", "Could not create topic.");
        return View(vm);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var resp = await client.GetAsync($"/api/topics/{id}");
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        var json = await resp.Content.ReadAsStringAsync();
        var vm = JsonSerializer.Deserialize<TopicVm>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, TopicVm vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var json = JsonSerializer.Serialize(vm);
        var resp = await client.PutAsync($"/api/topics/{id}",
            new StringContent(json, Encoding.UTF8, "application/json"));

        if (resp.IsSuccessStatusCode)
            return RedirectToAction(nameof(Manage));

        ModelState.AddModelError("", "Update failed.");
        return View(vm);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var resp = await client.GetAsync($"/api/topics/{id}");
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        var json = await resp.Content.ReadAsStringAsync();
        var vm = JsonSerializer.Deserialize<TopicVm>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        await client.DeleteAsync($"/api/topics/{id}");
        return RedirectToAction(nameof(Manage));
    }
}

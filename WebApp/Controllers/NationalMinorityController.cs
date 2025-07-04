using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WebApp.ViewModels;

public class NationalMinorityController : Controller
{
    private readonly IHttpClientFactory _httpFactory;

    public NationalMinorityController(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    // Public-facing list
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var resp = await client.GetAsync("/api/nationalminorities");
        if (!resp.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not load national minorities.";
            return View(new List<NationalMinorityVm>());
        }

        var json = await resp.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<NationalMinorityVm>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(data);
    }

    // Admin listing / management
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var resp = await client.GetAsync("/api/nationalminorities");
        var json = await resp.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<NationalMinorityVm>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(data);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new NationalMinorityVm());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(NationalMinorityVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var json = JsonSerializer.Serialize(vm);
        var resp = await client.PostAsync("/api/nationalminorities",
            new StringContent(json, Encoding.UTF8, "application/json"));

        if (resp.IsSuccessStatusCode)
            return RedirectToAction(nameof(Manage));

        ModelState.AddModelError("", "Failed to create minority.");
        return View(vm);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var resp = await client.GetAsync($"/api/nationalminorities/{id}");
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        var json = await resp.Content.ReadAsStringAsync();
        var vm = JsonSerializer.Deserialize<NationalMinorityVm>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, NationalMinorityVm vm)
    {
        if (id != vm.Id)
            return BadRequest();

        if (!ModelState.IsValid) return View(vm);

        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var json = JsonSerializer.Serialize(vm);
        var resp = await client.PutAsync($"/api/nationalminorities/{id}",
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

        var resp = await client.GetAsync($"/api/nationalminorities/{id}");
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        var json = await resp.Content.ReadAsStringAsync();
        var vm = JsonSerializer.Deserialize<NationalMinorityVm>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        await client.DeleteAsync($"/api/nationalminorities/{id}");
        return RedirectToAction(nameof(Manage));
    }
}

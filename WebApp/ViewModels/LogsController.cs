using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using WebApp.ViewModels;

[Authorize(Roles = "Admin")]
public class LogsController : Controller
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly AuthService _authService;

    public LogsController(IHttpClientFactory httpFactory, AuthService authService)
    {
        _httpFactory = httpFactory;
        _authService = authService;
    }

    // GET: /Logs
    public async Task<IActionResult> Index()
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");
        _authService.AttachToken(client, token);

        var response = await client.GetAsync("/api/logs");
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Unable to retrieve logs.";
            return View(new List<LogEntryVm>());
        }

        var json = await response.Content.ReadAsStringAsync();
        var logs = JsonSerializer.Deserialize<List<LogEntryVm>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(logs);
    }
}

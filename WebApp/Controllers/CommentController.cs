using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;
using WebApp.ViewModels;

public class CommentController : Controller
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly AuthService _authService;

    public CommentController(IHttpClientFactory httpFactory, AuthService authService)
    {
        _httpFactory = httpFactory;
        _authService = authService;
    }

    // GET: /Comment/MyComments
    [Authorize]
    public async Task<IActionResult> MyComments()
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var response = await client.GetAsync("/api/comments/mine");
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Unable to fetch your comments.";
            return View(new List<CommentVm>());
        }

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<CommentVm>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(data);
    }

    // POST: /Comment/Add (used from CulturalHeritage/Details)
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add(CreateCommentVm vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid comment submission.";
            return RedirectToAction("Details", "CulturalHeritage", new { id = vm.HeritageId });
        }

        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var json = JsonSerializer.Serialize(vm);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var resp = await client.PostAsync("/api/comments", content);
        if (!resp.IsSuccessStatusCode)
            TempData["Error"] = "Failed to submit comment.";
        else
            TempData["Success"] = "Comment submitted and pending approval.";

        return RedirectToAction("Details", "CulturalHeritage", new { id = vm.HeritageId });
    }

    // GET: /Comment/PendingApproval
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PendingApproval()
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        var response = await client.GetAsync("/api/comments/pending");
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Unable to load pending comments.";
            return View(new List<CommentVm>());
        }

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<CommentVm>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(data);
    }

    // POST: /Comment/Approve/5
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        await client.PostAsync($"/api/comments/{id}/approve", null);
        return RedirectToAction(nameof(PendingApproval));
    }

    // POST: /Comment/Reject/5
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(int id)
    {
        var client = _httpFactory.CreateClient("ApiClient");
        var token = await HttpContext.GetTokenAsync("access_token");

        await client.PostAsync($"/api/comments/{id}/reject", null);
        return RedirectToAction(nameof(PendingApproval));
    }
}

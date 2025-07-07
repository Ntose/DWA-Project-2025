// File: WebApp/Controllers/AdminController.cs

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _http;
        public AdminController(IHttpClientFactory http) => _http = http;

        // GET /Admin or /Admin/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await BuildAdminViewModel();
            return View("AdminTerminal", vm);
        }

        // GET /Admin/AdminTerminal
        [HttpGet]
        public async Task<IActionResult> AdminTerminal()
        {
            var vm = await BuildAdminViewModel();
            return View(vm);
        }

        // GET /Admin/CreateCulturalHeritage
        [HttpGet]
        public async Task<IActionResult> CreateCulturalHeritage()
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var minorities = await FetchListAsync<NationalMinorityViewModel>(client, "NationalMinority");
            var topics = await FetchListAsync<TopicViewModel>(client, "Topic");

            var vm = new CulturalHeritageEditViewModel
            {
                Minorities = minorities,
                Topics = topics
            };
            return View(vm);
        }

        // POST /Admin/CreateCulturalHeritage
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCulturalHeritage(CulturalHeritageEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return await ReloadCreateForm(vm, null);

            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var payload = new
            {
                vm.Name,
                vm.Description,
                vm.ImageUrl,
                vm.NationalMinorityId,
                vm.TopicIds
            };

            var resp = await client.PostAsJsonAsync("CulturalHeritage", payload);
            if (resp.IsSuccessStatusCode)
                return RedirectToAction(nameof(AdminTerminal));

            ModelState.AddModelError(string.Empty, await resp.Content.ReadAsStringAsync());
            return await ReloadCreateForm(vm, client);
        }

        // GET /Admin/CreateNationalMinority
        [HttpGet]
        public IActionResult CreateNationalMinority() =>
            View(new NationalMinorityViewModel());

        // POST /Admin/CreateNationalMinority
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNationalMinority(NationalMinorityViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var resp = await client.PostAsJsonAsync("NationalMinority", new { vm.Name });
            if (resp.IsSuccessStatusCode)
                return RedirectToAction(nameof(AdminTerminal));

            ModelState.AddModelError(string.Empty, await resp.Content.ReadAsStringAsync());
            return View(vm);
        }

        // GET /Admin/CreateTopic
        [HttpGet]
        public IActionResult CreateTopic() =>
            View(new TopicViewModel());

        // POST /Admin/CreateTopic
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTopic(TopicViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var resp = await client.PostAsJsonAsync("Topic", new { vm.Name });
            if (resp.IsSuccessStatusCode)
                return RedirectToAction(nameof(AdminTerminal));

            ModelState.AddModelError(string.Empty, await resp.Content.ReadAsStringAsync());
            return View(vm);
        }

        // ── PRIVATE HELPERS ────────────────────────────────────

        private async Task<AdminViewModel> BuildAdminViewModel()
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            // Fetch users
            var users = await FetchListAsync<UserViewModel>(client, "User");

            // Fetch log count
            var count = await client.GetFromJsonAsync<int>("Logs/count");

            // Fetch recent logs
            var logs = await FetchListAsync<LogViewModel>(client, "Logs/get/50");

            return new AdminViewModel
            {
                Users = users,
                LogCount = count,
                Logs = logs
            };
        }

        private async Task<List<T>> FetchListAsync<T>(HttpClient client, string url)
        {
            var resp = await client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return new List<T>();

            var raw = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("$values", out var inner))
            {
                root = inner;
            }

            if (root.ValueKind != JsonValueKind.Array)
                return new List<T>();

            return JsonSerializer.Deserialize<List<T>>(
                root.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<T>();
        }

        private async Task<IActionResult> ReloadCreateForm(
            CulturalHeritageEditViewModel vm,
            HttpClient? client)
        {
            var httpClient = client ?? _http.CreateClient("DataAPI");
            AttachBearerToken(httpClient);

            vm.Minorities = await FetchListAsync<NationalMinorityViewModel>(httpClient, "NationalMinority");
            vm.Topics = await FetchListAsync<TopicViewModel>(httpClient, "Topic");
            return View(vm);
        }

        private void AttachBearerToken(HttpClient client)
        {
            var jwt = User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        }
    }
}

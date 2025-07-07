using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize] // Require authentication for all actions unless overridden
    public class CulturalHeritageController : Controller
    {
        private readonly IHttpClientFactory _http;
        public CulturalHeritageController(IHttpClientFactory http) => _http = http;

        // GET: /CulturalHeritage
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            List<CulturalHeritageListViewModel> list;

            try
            {
                var resp = await client.GetAsync("CulturalHeritage");
                if (!resp.IsSuccessStatusCode)
                    return View(new List<CulturalHeritageListViewModel>());

                using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                var root = UnwrapValues(doc.RootElement);

                // Map JSON to view models
                list = root.ValueKind == JsonValueKind.Array
                    ? root.EnumerateArray().Select(el => new CulturalHeritageListViewModel
                    {
                        Id = el.GetProperty("id").GetInt32(),
                        Name = el.GetProperty("name").GetString() ?? "",
                        MinorityName = el.GetProperty("nationalMinority")
                                          .GetProperty("name").GetString() ?? "",
                        Topics = ExtractTopics(el.GetProperty("topics"))
                    }).ToList()
                    : new List<CulturalHeritageListViewModel>();
            }
            catch
            {
                list = new List<CulturalHeritageListViewModel>();
            }

            return View(list);
        }

        // GET: /CulturalHeritage/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            // Fetch cultural heritage details
            var resp = await client.GetAsync($"CulturalHeritage/{id}");
            if (!resp.IsSuccessStatusCode)
                return NotFound();

            CulturalHeritageDetailsViewModel vm;
            using (var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync()))
            {
                var r = doc.RootElement;
                vm = new CulturalHeritageDetailsViewModel
                {
                    Id = r.GetProperty("id").GetInt32(),
                    Name = r.GetProperty("name").GetString() ?? "",
                    Description = r.GetProperty("description").GetString() ?? "",
                    ImageUrl = r.GetProperty("imageUrl").GetString() ?? "",
                    MinorityName = r.GetProperty("nationalMinority")
                                    .GetProperty("name").GetString() ?? "",
                    Themes = ExtractTopics(r.GetProperty("topics"))
                };
            }

            // Load associated comments
            vm.Comments = await FetchListAsync<CommentViewModel>(
                client,
                $"CulturalHeritage/{id}/Comments"
            );

            // Prepare model for new comment submission
            vm.NewComment = new CommentCreateViewModel();

            return View(vm);
        }

        // POST: /CulturalHeritage/PostComment/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PostComment(int id, CommentCreateViewModel newComment)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Details), new { id });

            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            try
            {
                await client.PostAsJsonAsync(
                    $"CulturalHeritage/{id}/Comments",
                    new { Text = newComment.Text }
                );
            }
            catch
            {
                // Ignore errors silently
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /CulturalHeritage/Create
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            // Load dropdown data for form
            var vm = new CulturalHeritageEditViewModel
            {
                Minorities = await FetchListAsync<NationalMinorityViewModel>(client, "NationalMinority"),
                Topics = await FetchListAsync<TopicViewModel>(client, "Topic")
            };
            return View(vm);
        }

        // POST: /CulturalHeritage/Create
        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CulturalHeritageEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return await ReloadForm(vm);

            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var resp = await client.PostAsJsonAsync("CulturalHeritage", new
            {
                vm.Name,
                vm.Description,
                vm.ImageUrl,
                vm.NationalMinorityId,
                vm.TopicIds
            });

            if (resp.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", await resp.Content.ReadAsStringAsync());
            return await ReloadForm(vm);
        }

        // GET: /CulturalHeritage/Edit/5
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var resp = await client.GetAsync($"CulturalHeritage/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var r = doc.RootElement;

            var vm = new CulturalHeritageEditViewModel
            {
                Id = id,
                Name = r.GetProperty("name").GetString() ?? "",
                Description = r.GetProperty("description").GetString() ?? "",
                ImageUrl = r.GetProperty("imageUrl").GetString() ?? "",
                NationalMinorityId = r.GetProperty("nationalMinority").GetProperty("id").GetInt32(),
                TopicIds = ExtractTopicIds(r.GetProperty("topics")),
                Minorities = await FetchListAsync<NationalMinorityViewModel>(client, "NationalMinority"),
                Topics = await FetchListAsync<TopicViewModel>(client, "Topic")
            };

            return View(vm);
        }

        // POST: /CulturalHeritage/Edit/5
        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CulturalHeritageEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return await ReloadForm(vm);

            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var resp = await client.PutAsJsonAsync(
                $"CulturalHeritage/{vm.Id}", new
                {
                    vm.Name,
                    vm.Description,
                    vm.ImageUrl,
                    vm.NationalMinorityId,
                    vm.TopicIds
                });

            if (resp.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", await resp.Content.ReadAsStringAsync());
            return await ReloadForm(vm);
        }

        // POST: /CulturalHeritage/Delete/5
        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);
            await client.DeleteAsync($"CulturalHeritage/{id}");
            return RedirectToAction(nameof(Index));
        }

        // ── HELPERS ────────────────────────────────────────────

        // Adds JWT to request headers
        private void AttachBearerToken(HttpClient client)
        {
            var jwt = User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        }

        // Handles $values wrapping in JSON responses
        private static JsonElement UnwrapValues(JsonElement el)
            => el.ValueKind == JsonValueKind.Object && el.TryGetProperty("$values", out var inner)
                ? inner : el;

        // Extracts topic names from JSON
        private static List<string> ExtractTopics(JsonElement topicsElem)
        {
            var arr = UnwrapValues(topicsElem);
            if (arr.ValueKind != JsonValueKind.Array)
                return new List<string>();

            return arr.EnumerateArray()
                      .Select(item =>
                          item.TryGetProperty("topic", out var tObj)
                            ? tObj.GetProperty("name").GetString() ?? ""
                            : item.GetProperty("name").GetString() ?? ""
                      ).ToList();
        }

        // Extracts topic IDs from JSON
        private static List<int> ExtractTopicIds(JsonElement topicsElem)
        {
            var arr = UnwrapValues(topicsElem);
            if (arr.ValueKind != JsonValueKind.Array)
                return new List<int>();

            return arr.EnumerateArray()
                      .Select(item =>
                          item.TryGetProperty("topic", out var tObj)
                            ? tObj.GetProperty("id").GetInt32()
                            : item.GetProperty("id").GetInt32()
                      ).ToList();
        }
        // Fetches a list of items from the API and deserializes them
        private async Task<List<T>> FetchListAsync<T>(HttpClient client, string url)
        {
            var resp = await client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return new List<T>();

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var arr = UnwrapValues(doc.RootElement);

            if (arr.ValueKind != JsonValueKind.Array)
                return new List<T>();

            return JsonSerializer.Deserialize<List<T>>(
                arr.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<T>();
        }

        // Reloads form data (dropdowns) after a failed create/edit submission
        private async Task<IActionResult> ReloadForm(CulturalHeritageEditViewModel vm)
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            vm.Minorities = await FetchListAsync<NationalMinorityViewModel>(client, "NationalMinority");
            vm.Topics = await FetchListAsync<TopicViewModel>(client, "Topic");

            return View(vm);
        }
    }
}
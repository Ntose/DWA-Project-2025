// File: WebApp/Controllers/CulturalHeritageController.cs
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
    [Authorize]                    // require login
    public class CulturalHeritageController : Controller
    {
        private readonly IHttpClientFactory _http;
        public CulturalHeritageController(IHttpClientFactory http)
            => _http = http;

        // GET: /CulturalHeritage
        public async Task<IActionResult> Index()
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            // fetch raw JSON
            var raw = await client.GetStringAsync("CulturalHeritage");
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;
            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("$values", out var vals))
                root = vals;

            var list = JsonSerializer.Deserialize<List<CulturalHeritageListViewModel>>(
                root.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<CulturalHeritageListViewModel>();

            return View(list);
        }

        // GET: /CulturalHeritage/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var resp = await client.GetAsync($"CulturalHeritage/{id}");
            if (!resp.IsSuccessStatusCode)
                return NotFound();

            // manually build DetailsViewModel
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var r = doc.RootElement;

            var vm = new CulturalHeritageDetailsViewModel
            {
                Id = r.GetProperty("id").GetInt32(),
                Name = r.GetProperty("name").GetString()!,
                Description = r.GetProperty("description").GetString() ?? "",
                ImageUrl = r.GetProperty("imageUrl").GetString() ?? "",
                MinorityName = r.GetProperty("nationalMinority").GetProperty("name").GetString()!,
                Topics = r.GetProperty("topics")
                                 .EnumerateArray()
                                 .Select(t => t.GetProperty("topic").GetProperty("name").GetString()!)
            };

            return View(vm);
        }

        // GET: /CulturalHeritage/Create
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var vm = new CulturalHeritageEditViewModel
            {
                Minorities = await FetchListAsync<NationalMinorityViewModel>(client, "NationalMinority"),
                Topics = await FetchListAsync<TopicViewModel>(client, "Topic")
            };
            return View(vm);
        }

        // POST: /CulturalHeritage/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(CulturalHeritageEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return await ReloadForm(vm);

            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var payload = new
            {
                Name = vm.Name,
                Description = vm.Description,
                ImageUrl = vm.ImageUrl,
                NationalMinorityId = vm.NationalMinorityId,
                TopicIds = vm.TopicIds
            };

            var resp = await client.PostAsJsonAsync("CulturalHeritage", payload);
            if (resp.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            await HandleErrors(vm, resp);
            return View(vm);
        }

        // GET: /CulturalHeritage/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var resp = await client.GetAsync($"CulturalHeritage/{id}");
            if (!resp.IsSuccessStatusCode)
                return NotFound();

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var r = doc.RootElement;

            var vm = new CulturalHeritageEditViewModel
            {
                Id = id,
                Name = r.GetProperty("name").GetString()!,
                Description = r.GetProperty("description").GetString() ?? "",
                ImageUrl = r.GetProperty("imageUrl").GetString() ?? "",
                NationalMinorityId = r.GetProperty("nationalMinority").GetProperty("id").GetInt32(),
                TopicIds = r.GetProperty("topics")
                                       .EnumerateArray()
                                       .Select(t => t.GetProperty("topic").GetProperty("id").GetInt32())
                                       .ToList(),
                Minorities = await FetchListAsync<NationalMinorityViewModel>(client, "NationalMinority"),
                Topics = await FetchListAsync<TopicViewModel>(client, "Topic")
            };
            return View(vm);
        }

        // POST: /CulturalHeritage/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(CulturalHeritageEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return await ReloadForm(vm);

            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var payload = new
            {
                Name = vm.Name,
                Description = vm.Description,
                ImageUrl = vm.ImageUrl,
                NationalMinorityId = vm.NationalMinorityId,
                TopicIds = vm.TopicIds
            };

            var resp = await client.PutAsJsonAsync($"CulturalHeritage/{vm.Id}", payload);
            if (resp.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            await HandleErrors(vm, resp);
            return View(vm);
        }

        // POST: /CulturalHeritage/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);
            await client.DeleteAsync($"CulturalHeritage/{id}");
            return RedirectToAction(nameof(Index));
        }

        // ── helpers ───────────────────────────────────────────────

        private void AttachBearerToken(HttpClient client)
        {
            var jwt = User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        }

        private async Task<List<T>> FetchListAsync<T>(HttpClient client, string url)
        {
            var raw = await client.GetStringAsync(url);
            using var jd = JsonDocument.Parse(raw);
            var el = jd.RootElement;
            if (el.ValueKind == JsonValueKind.Object
             && el.TryGetProperty("$values", out var inner))
                el = inner;

            return JsonSerializer.Deserialize<List<T>>(
                el.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<T>();
        }

        private async Task<IActionResult> ReloadForm(CulturalHeritageEditViewModel vm)
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);
            vm.Minorities = await FetchListAsync<NationalMinorityViewModel>(client, "NationalMinority");
            vm.Topics = await FetchListAsync<TopicViewModel>(client, "Topic");
            return View(vm);
        }

        private async Task HandleErrors(CulturalHeritageEditViewModel vm, HttpResponseMessage resp)
        {
            var raw = await resp.Content.ReadAsStringAsync();
            try
            {
                var prob = JsonSerializer.Deserialize<ValidationProblemDetails>(
                    raw,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                foreach (var kv in prob.Errors)
                    foreach (var e in kv.Value)
                        ModelState.AddModelError(kv.Key, e);
            }
            catch
            {
                ModelState.AddModelError("", raw);
            }
        }
    }
}

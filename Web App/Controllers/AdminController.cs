// File: WebApp/Controllers/AdminController.cs

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
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

        // GET /Admin
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

            // MANUALLY fetch NationalMinority list
            var minorities = await FetchListAsync<NationalMinorityViewModel>(client, "NationalMinority");

            // MANUALLY fetch Topic list
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
                return await ReloadCreateForm(vm, vm);

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

            ModelState.AddModelError(string.Empty, await resp.Content.ReadAsStringAsync());
            return await ReloadCreateForm(vm, client);
        }

        // GET /Admin/CreateNationalMinority
        [HttpGet]
        public IActionResult CreateNationalMinority()
            => View(new NationalMinorityViewModel());

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
                return RedirectToAction(nameof(CreateCulturalHeritage));

            ModelState.AddModelError(string.Empty, await resp.Content.ReadAsStringAsync());
            return View(vm);
        }

        // GET /Admin/CreateTopic
        [HttpGet]
        public IActionResult CreateTopic()
            => View(new TopicViewModel());

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
                return RedirectToAction(nameof(CreateCulturalHeritage));

            ModelState.AddModelError(string.Empty, await resp.Content.ReadAsStringAsync());
            return View(vm);
        }

        // -------------------
        // PRIVATE HELPERS
        // -------------------

        // Fetch any List<T> from an endpoint that might return {"$values":[...]} or [...] directly
        private async Task<List<T>> FetchListAsync<T>(HttpClient client, string url)
        {
            var list = new List<T>();
            var resp = await client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return list;

            var raw = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            // if wrapper object with $values
            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("$values", out var inner))
            {
                root = inner;
            }

            if (root.ValueKind == JsonValueKind.Array)
            {
                list = JsonSerializer.Deserialize<List<T>>(
                    root.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? list;
            }

            return list;
        }

        // Re-load form data on POST error
        private async Task<IActionResult> ReloadCreateForm(CulturalHeritageEditViewModel vm, HttpClient client)
        {
            // vm passed as second param means we've already got the client
            vm.Minorities = await FetchListAsync<NationalMinorityViewModel>(client, "NationalMinority");
            vm.Topics = await FetchListAsync<TopicViewModel>(client, "Topic");
            return View(vm);
        }

        // Overload to pass vm and factory
        private async Task<IActionResult> ReloadCreateForm(CulturalHeritageEditViewModel vm, CulturalHeritageEditViewModel _)
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);
            return await ReloadCreateForm(vm, client);
        }

        // Build Users + LogCount + Logs for Index/AdminTerminal
        private async Task<AdminViewModel> BuildAdminViewModel()
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var vm = new AdminViewModel();

            // Users
            vm.Users = await FetchListAsync<UserViewModel>(client, "User");

            // LogCount
            var cResp = await client.GetAsync("Logs/count");
            if (cResp.IsSuccessStatusCode)
                vm.LogCount = await cResp.Content.ReadFromJsonAsync<int>();

            // Logs
            vm.Logs = await FetchListAsync<LogViewModel>(client, "Logs/get/50");

            return vm;
        }

        // Grab the JWT from the cookie-principal and apply
        private void AttachBearerToken(HttpClient client)
        {
            var jwt = User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        }
        

    }
}

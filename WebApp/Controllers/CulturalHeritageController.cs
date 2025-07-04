using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using WebApp.ViewModels;


namespace WebApp.Controllers
{
    public class CulturalHeritageController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public CulturalHeritageController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        // GET: /CulturalHeritage?page=1&count=10
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1, int count = 10)
        {
            var client = _httpFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/culturalheritage/search?page={page}&count={count}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load heritage items.";
                return View(new List<HeritageListItemVm>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<HeritageListItemVm>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(items);
        }

        // GET: /CulturalHeritage/Search?term=xxx&page=1&count=10
        [AllowAnonymous]
        public async Task<IActionResult> Search(string term, int page = 1, int count = 10)
        {
            var client = _httpFactory.CreateClient("ApiClient");
            var url = $"/api/culturalheritage/search?page={page}&count={count}";
            if (!string.IsNullOrWhiteSpace(term))
                url += $"&term={Uri.EscapeDataString(term)}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return View("Index", new List<HeritageListItemVm>());

            var json = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<HeritageListItemVm>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View("Index", items);
        }

        // GET: /CulturalHeritage/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/culturalheritage/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<HeritageDetailsVm>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(model);
        }

        // GET: /CulturalHeritage/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new CreateHeritageVm());
        }

        // POST: /CulturalHeritage/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateHeritageVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var client = _httpFactory.CreateClient("ApiClient");
            var token = await HttpContext.GetTokenAsync("access_token");

            var payload = JsonSerializer.Serialize(vm);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/culturalheritage", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to create item.");
            return View(vm);
        }

        // GET: /CulturalHeritage/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpFactory.CreateClient("ApiClient");
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await client.GetAsync($"/api/culturalheritage/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<EditHeritageVm>(json,
                         new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(vm);
        }

        // POST: /CulturalHeritage/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, EditHeritageVm vm)
        {
            if (id != vm.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(vm);

            var client = _httpFactory.CreateClient("ApiClient");
            var token = await HttpContext.GetTokenAsync("access_token");

            var payload = JsonSerializer.Serialize(vm);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/culturalheritage/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to update item.");
            return View(vm);
        }

        // GET: /CulturalHeritage/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpFactory.CreateClient("ApiClient");
            var token = await HttpContext.GetTokenAsync("access_token");

            var response = await client.GetAsync($"/api/culturalheritage/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<HeritageDetailsVm>(json,
                         new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(vm);
        }

        // POST: /CulturalHeritage/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpFactory.CreateClient("ApiClient");
            var token = await HttpContext.GetTokenAsync("access_token");

            await client.DeleteAsync($"/api/culturalheritage/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}

// File: WebApp/Controllers/AdminController.cs

using System.Collections.Generic;
using System.Net;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            // 1) Load users
            var users = new List<UserViewModel>();
            var uResp = await client.GetAsync("User");
            if (uResp.IsSuccessStatusCode)
            {
                var raw = await uResp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;
                JsonElement arr = root.ValueKind == JsonValueKind.Object &&
                                  root.TryGetProperty("$values", out var v)
                    ? v
                    : root.ValueKind == JsonValueKind.Array
                        ? root
                        : default;

                if (arr.ValueKind == JsonValueKind.Array)
                {
                    users = JsonSerializer.Deserialize<List<UserViewModel>>(
                        arr.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
            }

            // 2) Load total log count
            int count = 0;
            var cResp = await client.GetAsync("Logs/count");
            if (cResp.IsSuccessStatusCode)
            {
                count = await cResp.Content.ReadFromJsonAsync<int>();
            }

            // 3) Load latest N logs
            const int N = 50;
            var logs = new List<LogViewModel>();
            var lResp = await client.GetAsync($"Logs/get/{N}");
            if (lResp.IsSuccessStatusCode)
            {
                var raw = await lResp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;
                JsonElement arr = root.ValueKind == JsonValueKind.Object &&
                                  root.TryGetProperty("$values", out var v)
                    ? v
                    : root.ValueKind == JsonValueKind.Array
                        ? root
                        : default;

                if (arr.ValueKind == JsonValueKind.Array)
                {
                    logs = JsonSerializer.Deserialize<List<LogViewModel>>(
                        arr.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
            }

            var vm = new AdminViewModel
            {
                Users = users,
                LogCount = count,
                Logs = logs
            };

            return View(vm);
        }

        // Attach JWT bearer token from current user to outgoing requests
        private void AttachBearerToken(HttpClient client)
        {
            var jwt = User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        }
    }
}

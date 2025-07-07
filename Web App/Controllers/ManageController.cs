using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly IHttpClientFactory _http;
        public ManageController(IHttpClientFactory http) => _http = http;

        // GET: /Manage or /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _http.CreateClient("DataAPI");

            var jwt = User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var vm = await client
                .GetFromJsonAsync<ManageViewModel>("User/profile");

            if (vm == null)
                return Challenge();

            return View("Manage", vm);
        }

        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword() => View();
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _http;
        public AccountController(IHttpClientFactory http) => _http = http;

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string returnUrl = null) =>
            View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var authClient = _http.CreateClient("AuthAPI");
            var authResp = await authClient.PostAsJsonAsync("login", new { vm.Username, vm.Password });
            var authRaw = await authResp.Content.ReadAsStringAsync();

            if (!authResp.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"Login failed {(int)authResp.StatusCode}: {authRaw}");
                return View(vm);
            }

            var authData = JsonSerializer.Deserialize<Dictionary<string, string>>(authRaw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (authData == null || !authData.TryGetValue("token", out var jwt))
            {
                ModelState.AddModelError(string.Empty, "Invalid login response.");
                return View(vm);
            }

            var dataClient = _http.CreateClient("DataAPI");
            dataClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var profileResp = await dataClient.GetAsync("User/profile");
            if (!profileResp.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"Failed to load profile: {profileResp.StatusCode}");
                return View(vm);
            }

            var profileRaw = await profileResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(profileRaw);
            var root = doc.RootElement;

            var userId = root.GetProperty("id").GetInt32().ToString();
            var username = root.GetProperty("username").GetString() ?? vm.Username;
            var role = root.GetProperty("role").GetString() ?? "User";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("JWT", jwt)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = vm.RememberMe });

            if (string.IsNullOrEmpty(vm.ReturnUrl) || !Url.IsLocalUrl(vm.ReturnUrl))
                return RedirectToAction("Index", "Home");

            return LocalRedirect(vm.ReturnUrl);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult AccessDenied() => View();

        [HttpGet, AllowAnonymous]
        public IActionResult Register(string returnUrl = null) =>
            View(new RegisterViewModel { ReturnUrl = returnUrl });

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var client = _http.CreateClient("AuthAPI");
            var resp = await client.PostAsJsonAsync("register", new
            {
                vm.Username,
                vm.FirstName,
                vm.LastName,
                vm.Phone,
                vm.Email,
                vm.Password,
                vm.ConfirmPassword
            });

            var raw = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                TryAddJsonErrors(raw);
                ModelState.AddModelError(string.Empty, $"Registration failed {(int)resp.StatusCode}: {raw}");
                return View(vm);
            }

            return RedirectToAction("Login", new { returnUrl = vm.ReturnUrl });
        }

        [HttpPost, Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Manage()
        {
            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var resp = await client.GetAsync("User/profile");
            var raw = await resp.Content.ReadAsStringAsync();
            ViewBag.RawProfile = raw;

            if (resp.StatusCode == HttpStatusCode.Unauthorized)
                return RedirectToAction("Login");

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"Error {(int)resp.StatusCode}: {resp.ReasonPhrase}");
                return View(new ManageViewModel());
            }

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            var vm = new ManageViewModel
            {
                Username = root.GetProperty("username").GetString(),
                Email = root.GetProperty("email").GetString(),
                FirstName = root.GetProperty("firstName").GetString(),
                LastName = root.GetProperty("lastName").GetString(),
                Phone = root.GetProperty("phone").GetString(),
                DateRegistered = root.GetProperty("dateRegistered").GetDateTime(),
                Role = root.GetProperty("role").GetString(),
            };

            if (root.TryGetProperty("comments", out var cObj) &&
                cObj.TryGetProperty("$values", out var arr))
            {
                vm.Comments = JsonSerializer.Deserialize<List<ManageCommentViewModel>>(
                    arr.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }

            return View(vm);
        }

        [HttpGet, Authorize]
        public IActionResult ChangePassword() =>
            View(new ChangePasswordViewModel());

        [HttpPost, Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var client = _http.CreateClient("AuthAPI");
            AttachBearerToken(client);

            var resp = await client.PostAsJsonAsync("changepassword", new
            {
                vm.OldPassword,
                vm.NewPassword
            });

            var raw = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                TryAddJsonErrors(raw);
                ModelState.AddModelError(string.Empty,
                    !string.IsNullOrWhiteSpace(raw) ? raw : $"Error {(int)resp.StatusCode}: {resp.ReasonPhrase}");
                return View(vm);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["PasswordChanged"] = "Your password has been changed. Please log in again.";
            return RedirectToAction("Login");
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetApiToken()
        {
            try
            {
                var username = User.Identity.Name;

                var loginPayload = new { username };

                var client = HttpContext.RequestServices
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient("AuthAPI");

                var json = JsonSerializer.Serialize(loginPayload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenData = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                    return Ok(new { token = tokenData?.Token });
                }

                return BadRequest(new { message = "Failed to get API token" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> UpdateProfile()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
                return BadRequest("Empty body.");

            JsonDocument json;
            try
            {
                json = JsonDocument.Parse(body);
            }
            catch
            {
                return BadRequest("Invalid JSON.");
            }

            var client = _http.CreateClient("DataAPI");
            AttachBearerToken(client);

            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var resp = await client.PutAsync("User/profile", content);

            var raw = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return StatusCode((int)resp.StatusCode, string.IsNullOrWhiteSpace(raw)
                    ? "Error updating profile."
                    : raw);
            }

            return Ok();
        }

        private void AttachBearerToken(HttpClient client)
        {
            var jwt = User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(jwt))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwt);
            }
        }

        private void TryAddJsonErrors(string rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson) ||
                !rawJson.TrimStart().StartsWith("{"))
                return;

            try
            {
                var details = JsonSerializer.Deserialize<ValidationProblemDetails>(
                    rawJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (details?.Errors != null)
                {
                    foreach (var kv in details.Errors)
                    {
                        foreach (var err in kv.Value)
                        {
                            ModelState.AddModelError(kv.Key, err);
                        }
                    }
                }
            }
            catch
            {
                // Ignore JSON parsing errors
            }
        }

        public class TokenResponse
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
        }
    }
}
            
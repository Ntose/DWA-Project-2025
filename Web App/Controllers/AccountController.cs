﻿// File: WebApp/Controllers/AccountController.cs

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
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

        // GET: /Account/Login
        [HttpGet, AllowAnonymous]
        public IActionResult Login(string returnUrl = null) =>
            View(new LoginViewModel { ReturnUrl = returnUrl });

        // POST: /Account/Login
        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // 1) Authenticate and get JWT
            var authClient = _http.CreateClient("AuthAPI");
            var authResp = await authClient.PostAsJsonAsync("login", new
            {
                vm.Username,
                vm.Password
            });

            var authRaw = await authResp.Content.ReadAsStringAsync();
            if (!authResp.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty,
                    $"Login failed {(int)authResp.StatusCode}: {authRaw}");
                return View(vm);
            }

            var authData = JsonSerializer.Deserialize<Dictionary<string, string>>(authRaw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (authData == null || !authData.TryGetValue("token", out var jwt))
            {
                ModelState.AddModelError(string.Empty, "Invalid login response.");
                return View(vm);
            }

            // 2) Retrieve user profile with the JWT
            var dataClient = _http.CreateClient("DataAPI");
            dataClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);

            var profileResp = await dataClient.GetAsync("User/profile");
            if (!profileResp.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty,
                    $"Failed to load profile: {profileResp.StatusCode}");
                return View(vm);
            }

            var profileRaw = await profileResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(profileRaw);
            var root = doc.RootElement;

            // Extract required fields
            var userId = root.GetProperty("id").GetInt32().ToString();
            var username = root.GetProperty("username").GetString() ?? vm.Username;
            var role = root.GetProperty("role").GetString() ?? "User";

            // 3) Build claims including Role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name,           username),
                new Claim(ClaimTypes.Role,           role),
                new Claim("JWT",                     jwt)
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = vm.RememberMe }
            );

            // 4) Redirect
            if (string.IsNullOrEmpty(vm.ReturnUrl) || !Url.IsLocalUrl(vm.ReturnUrl))
                return RedirectToAction("Index", "Home");

            return LocalRedirect(vm.ReturnUrl);
        }

        // GET: /Account/Register
        [HttpGet, AllowAnonymous]
        public IActionResult Register(string returnUrl = null) =>
            View(new RegisterViewModel { ReturnUrl = returnUrl });

        // POST: /Account/Register
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
                ModelState.AddModelError(string.Empty,
                    $"Registration failed {(int)resp.StatusCode}: {raw}");
                return View(vm);
            }

            return RedirectToAction("Login", new { returnUrl = vm.ReturnUrl });
        }

        // POST: /Account/Logout
        [HttpPost, Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Manage
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
                ModelState.AddModelError(string.Empty,
                    $"Error {(int)resp.StatusCode}: {resp.ReasonPhrase}");
                return View(new ManageViewModel());
            }

            // manual parse for comments
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            var vm2 = new ManageViewModel
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
                vm2.Comments = JsonSerializer.Deserialize<List<CommentViewModel>>(
                    arr.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }

            return View(vm2);
        }

        // GET: /Account/ChangePassword
        [HttpGet, Authorize]
        public IActionResult ChangePassword() =>
            View(new ChangePasswordViewModel());

        // POST: /Account/ChangePassword
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
                    !string.IsNullOrWhiteSpace(raw)
                        ? raw
                        : $"Error {(int)resp.StatusCode}: {resp.ReasonPhrase}");
                return View(vm);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["PasswordChanged"] =
                "Your password has been changed. Please log in again.";
            return RedirectToAction("Login");
        }

        // attach JWT from cookie
        private void AttachBearerToken(HttpClient client)
        {
            var jwt = User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwt);
        }

        // surfacing API validation errors
        private void TryAddJsonErrors(string rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson) ||
                !rawJson.TrimStart().StartsWith("{")) return;

            try
            {
                var details = JsonSerializer.Deserialize<ValidationProblemDetails>(
                    rawJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (details?.Errors != null)
                    foreach (var kv in details.Errors)
                        foreach (var err in kv.Value)
                            ModelState.AddModelError(kv.Key, err);
            }
            catch { }
        }
    }
}
